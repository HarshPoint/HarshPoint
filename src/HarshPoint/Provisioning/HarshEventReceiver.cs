using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshEventReceiver : HarshProvisioner
    {
        private LazyObjectMapping<HarshEventReceiver, EventReceiverDefinition> _map;

        public HarshEventReceiver()
        {
            _map.Map(x => x.EventType);
            _map.Map(x => x.ReceiverUrl);
            _map.Map(x => x.SequenceNumber);

            WriteRecord = CreateRecordWriter<EventReceiverDefinition>(
                () => $"{Name} {EventType} {ReceiverUrl}"
            );
        }

        [Parameter]
        [MandatoryWhenCreating]
        public EventReceiverType EventType { get; set; }

        [Parameter]
        public Boolean Force { get; set; }

        [DefaultFromContext]
        [Parameter(Mandatory = true)]
        public IResolve<List> Lists { get; set; }

        [Parameter(Mandatory = true)]
        public String Name { get; set; }

        [Parameter]
        public Int32? SequenceNumber { get; set; }

        [Parameter]
        [MandatoryWhenCreating]
        public String ReceiverUrl { get; set; }

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<List>(
                list => list.EventReceivers.Include(
                    erd => erd.ReceiverId,
                    erd => erd.ReceiverName
                ),
                list => list.RootFolder.ServerRelativeUrl
            );

            context.Include(
                _map.GetTargetExpressions()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (Force)
            {
                await DeleteWhereDifferent();
            }

            foreach (var tuple in Existing)
            {
                if (tuple.Item2 == null)
                {
                    await CreateReceiver(tuple.Item1);
                }
                else
                {
                    WriteRecord.AlreadyExists(
                        tuple.Item1.RootFolder.ServerRelativeUrl,
                        tuple.Item2
                    );
                }
            }

            await base.OnProvisioningAsync();
        }

        [NeverDeletesUserData]
        protected override async Task OnUnprovisioningAsync()
        {
            foreach (var tuple in Existing)
            {
                if (tuple.Item2 == null)
                {
                    WriteRecord.DidNotExist(
                        tuple.Item1.RootFolder.ServerRelativeUrl
                    );
                }
                else
                {
                    await DeleteReceiver(tuple);
                }
            }

            await base.OnUnprovisioningAsync();
        }

        private async Task CreateReceiver(List list)
        {
            ValidateMandatoryWhenCreatingParameters();

            var ci = new EventReceiverDefinitionCreationInformation()
            {
                EventType = EventType,
                ReceiverName = Name,
                ReceiverUrl = ReceiverUrl,
            };

            if (SequenceNumber.HasValue)
            {
                ci.SequenceNumber = SequenceNumber.Value;
            }

            var receiver = list.EventReceivers.Add(ci);

            ClientContext.Load(
                receiver,
                erd => erd.ReceiverName,
                erd => erd.ReceiverId
            );

            ClientContext.Load(
                receiver,
                _map.GetTargetExpressions()
            );

            await ClientContext.ExecuteQueryAsync();

            WriteRecord.Added(
                list.RootFolder.ServerRelativeUrl, 
                receiver
            );
        }

        private async Task DeleteWhereDifferent()
        {
            foreach (var tuple in Existing.Where(t => t.Item2 != null))
            {
                if (_map.WouldChange(this, tuple.Item2))
                {
                    await DeleteReceiver(tuple);
                }
            }
        }

        private async Task DeleteReceiver(
            Tuple<List, EventReceiverDefinition> tuple
        )
        {
            tuple.Item2.DeleteObject();
            await ClientContext.ExecuteQueryAsync();

            WriteRecord.Removed(
                tuple.Item1.RootFolder.ServerRelativeUrl,
                tuple.Item2.ReceiverName
            );
        }

        private IEnumerable<Tuple<List, EventReceiverDefinition>> Existing
            => (from list in Lists
                let receivers = list.EventReceivers.Where(erd =>
                    StringComparer.OrdinalIgnoreCase.Equals(
                        erd.ReceiverName, Name
                    )
                )
                from receiver in receivers.DefaultIfEmpty()
                select Tuple.Create(list, receiver))
            .ToImmutableArray();

        private RecordWriter<HarshProvisionerContext, EventReceiverDefinition> WriteRecord { get; }
    }
}
