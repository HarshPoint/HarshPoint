using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshSiteGroup : HarshProvisioner
    {
        private LazyObjectMapping<HarshSiteGroup, Group> _map;

        public HarshSiteGroup()
        {
            Map(g => g.Description);

            ExistingGroup = DeferredResolveBuilder.Create(
                () => Resolve.SiteGroup().ByName(Name)
            );

            WriteRecord = CreateRecordWriter<Group>(() => Name);
        }

        [Parameter]
        public String Description
        {
            get;
            set;
        }

        [Parameter(Mandatory = true)]
        public String Name
        {
            get;
            set;
        }

        private ObjectMapper<HarshSiteGroup, Group>.IEntryBuilder Map(
            Expression<Func<Group, Object>> targetProperty
        )
            => _map.Map(targetProperty);

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<Group>(
                g => g.Title
            );

            context.Include(
                _map.GetTargetExpressions()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingGroup.Value == null)
            {
                ValidateMandatoryWhenCreatingParameters();

                Group = Web.SiteGroups.Add(new GroupCreationInformation()
                {
                    Title = Name,
                    Description = Description,
                });

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Added(Group);
            }
            else
            {
                Group = ExistingGroup.Value;
                if (_map.Apply(WriteRecord, this, Group))
                {
                    Group.Update();
                }
                else
                {
                    WriteRecord.AlreadyExists(Group);
                }
            }
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (ExistingGroup.HasValue)
            {
                Web.SiteGroups.Remove(ExistingGroup.Value);

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Removed();
            }
            else
            {
                WriteRecord.DidNotExist();
            }
        }

        private Group Group { get; set; }

        private RecordWriter<HarshProvisionerContext, Group> WriteRecord
        {
            get;
        }

        internal IResolveSingleOrDefault<Group> ExistingGroup
        {
            get;
            set;
        }
    }
}
