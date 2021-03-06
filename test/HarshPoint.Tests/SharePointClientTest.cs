﻿using HarshPoint.Diagnostics;
using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Records;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public abstract class SharePointClientTest :
        SeriloggedTest
    {
        private readonly HarshDisposableBag _disposables = new HarshDisposableBag();

        public SharePointClientTest(ITestOutputHelper output)
            : base(output)
        {
            ManualResolver = new ClientObjectManualResolver(CreateResolveContext);

            var progressBuffer = new ProgressBuffer<HarshProvisionerRecord>();
            Output = progressBuffer.Reports;

            ClientContext = SharePointTestContext.Create();
            Context = new HarshProvisionerContext(ClientContext)
                .WithProgress(
                    new ProgressComposite<HarshProvisionerRecord>(
                        new ProgressSerilog<HarshProvisionerRecord>(
                            HarshLog.ForContext("ProvisionerOutput", true)
                        ),
                        progressBuffer
                    )
                );
        }

        public void AddDisposable(IDisposable disposable)
            => _disposables.Add(disposable);

        public void AddDisposable(Action action)
            => _disposables.Add(action);

        public HarshProvisionerContext Context { get; }
        public ClientContext ClientContext { get; }
        public IReadOnlyCollection<HarshProvisionerRecord> Output { get; }
        public Site Site => ClientContext.Site;
        public TaxonomySession TaxonomySession => Context.TaxonomySession;
        public Web Web => ClientContext.Web;
        public ClientObjectManualResolver ManualResolver { get; }

        protected virtual ClientObjectResolveContext CreateResolveContext()
            => new ClientObjectResolveContext(Context);

        public override void Dispose()
        {
            try
            {
                _disposables.Dispose();
                ClientContext.ExecuteQueryAsync().Wait();
            }
            catch (AggregateException exc)
            {
                foreach (var inner in exc.InnerExceptions)
                {
                    Logger.Error.Write(inner);
                }
            }

            ClientContext.Dispose();
            base.Dispose();
        }

        protected async Task<Field> CreateField(
            params Expression<Func<Field, Object>>[] retrievals
        )
        {
            var guid = Guid.NewGuid();

            Web.Fields.AddFieldAsXml(
                $"<Field ID='{guid}' Name='{guid:n}' Type='Text' />",
                addToDefaultView: false,
                options: AddFieldOptions.DefaultValue
            );

            var field = Web.Fields.GetById(guid);

            ClientContext.Load(field, retrievals);
            await ClientContext.ExecuteQueryAsync();

            RegisterForDeletion(field);
            return field;
        }

        protected async Task<List> CreateList(
            params Expression<Func<List, Object>>[] retrievals
        )
        {
            var guid = Guid.NewGuid();
            var name = guid.ToStringInvariant("n");

            var list = Web.Lists.Add(new ListCreationInformation()
            {
                TemplateType = (Int32)ListTemplateType.GenericList,
                Title = name,
                Url = $"Lists/{name}",
            });

            if (retrievals.Any())
            {
                ClientContext.Load(list, retrievals);
            }

            ClientContext.Load(list, l => l.Id);

            await ClientContext.ExecuteQueryAsync();

            RegisterForDeletion(list);
            return list;
        }

        protected async Task<Group> CreateSiteGroup(
            params Expression<Func<Group, Object>>[] retrievals
        )
        {
            var guid = Guid.NewGuid();
            var name = guid.ToStringInvariant("n");

            var group = Web.SiteGroups.Add(new GroupCreationInformation()
            {
                Title = name,
                Description = $"HarshPoint test group {name}",
            });

            if (retrievals.Any())
            {
                ClientContext.Load(group, retrievals);
            }

            ClientContext.Load(group, l => l.Id);

            await ClientContext.ExecuteQueryAsync();

            RegisterForDeletion(group);
            return group;
        }

        protected void RegisterForDeletion(ContentType ct)
        {
            if (ct == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(ct));
            }

            AddDisposable(ct.DeleteObject);
        }

        protected void RegisterForDeletion(Field f)
        {
            if (f == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(f));
            }

            AddDisposable(f.DeleteObject);
        }

        protected void RegisterForDeletion(List list)
        {
            if (list == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(list));
            }

            AddDisposable(list.DeleteObject);
        }

        protected void RegisterForDeletion(Group group)
        {
            if (group == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(group));
            }

            AddDisposable(() =>
            {
                ClientContext.Web.SiteGroups.Remove(group);
            });
        }

        protected ObjectRecord<T> LastObjectOutput<T>()
            => Output.OfType<ObjectRecord<T>>().Last();

        private static readonly HarshLogger Logger = HarshLog.ForContext<SharePointClientTest>();
    }
}
