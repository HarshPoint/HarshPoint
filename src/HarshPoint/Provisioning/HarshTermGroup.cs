using System;
using System.Threading.Tasks;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;

namespace HarshPoint.Provisioning
{
    public sealed class HarshTermGroup : HarshProvisioner
    {
        public HarshTermGroup()
        {
            TermStore = Resolve.TermStoreSiteCollectionDefault();

            ExistingTermGroup = DeferredResolveBuilder.Create(() =>
                TermStore.ValidateIsClientObjectResolveBuilder()
                .TermGroup().ById(Id)
            );

            WriteRecord = CreateRecordWriter<TermGroup>(
                () => Name ?? Id.ToStringInvariant()
            );

            ModifyChildrenContextState(
                () => TermGroup
            );
        }

        [Parameter]
        [MandatoryWhenCreating]
        public String Name { get; set; }

        [Parameter(Mandatory = true)]
        public Guid Id { get; set; }

        public IResolveSingle<TermStore> TermStore { get; set; }

        internal IResolveSingleOrDefault<TermGroup> ExistingTermGroup { get; set; }

        private RecordWriter<HarshProvisionerContext, TermGroup> WriteRecord { get; }

        private TermGroup TermGroup { get; set; }

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<TermGroup>(
                tg => tg.Name
            );

            context.Include<TermStore>(
                ts => ts.Name
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingTermGroup.HasValue)
            {
                TermGroup = ExistingTermGroup.Value;

                WriteRecord.AlreadyExists(
                    TermStore.Value.Name,
                    ExistingTermGroup.Value
                );
            }
            else
            {
                ValidateMandatoryWhenCreatingParameters();

                TermGroup = TermStore.Value.CreateGroup(Name, Id);

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Added(
                    TermStore.Value.Name,
                    TermGroup
                );
            }

            await base.OnProvisioningAsync();
        }
    }
}
