using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Threading.Tasks;
using HarshPoint.Provisioning.Implementation;
using static System.FormattableString;

namespace HarshPoint.Provisioning
{
    public sealed class HarshTermSet : HarshProvisioner
    {
        public HarshTermSet()
        {
            ExistingTermSet = DeferredResolveBuilder.Create(
                () => new ResolveTermGroupTermSet(
                    TermGroup.ValidateIsClientObjectResolveBuilder()
                ).ById(Id)
            );

            WriteRecord = CreateRecordWriter<TermSet>(
                () => Name ?? Id.ToStringInvariant()
            );

            ModifyChildrenContextState(
                () => TermSet
            );
        }

        [DefaultFromContext]
        [Parameter(Mandatory = true)]
        public IResolveSingle<TermGroup> TermGroup { get; set; }

        [Parameter]
        [MandatoryWhenCreating]
        public String Name { get; set; }

        [Parameter(Mandatory = true)]
        public Guid Id { get; set; }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<TermStore>(
                ts => ts.DefaultLanguage
            );

            context.Include<TermGroup>(
                tg => tg.Name,
                tg => tg.TermStore.Name,
                tg => tg.TermStore.DefaultLanguage
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingTermSet.HasValue)
            {
                TermSet = ExistingTermSet.Value;

                WriteRecord.AlreadyExists(
                    // TODO: retrievals not included on default from context?
                    // RecordContext,
                    ExistingTermSet.Value
                );
            }
            else
            {
                ValidateMandatoryWhenCreatingParameters();

                TermSet = TermGroup.Value.CreateTermSet(
                    Name,
                    Id,
                    TermGroup.Value.TermStore.DefaultLanguage
                );

                await ClientContext.ExecuteQueryAsync();

                // TODO: retrievals not included on default from context?
                WriteRecord.Added(/*RecordContext, */TermSet);
            }

            await base.OnProvisioningAsync();
        }

        internal IResolveSingleOrDefault<TermSet> ExistingTermSet { get; set; }

        private TermSet TermSet { get; set; }

        private String RecordContext
            => Invariant(
                $"{TermGroup.Value.TermStore.Name} / {TermGroup.Value.Name}"
            );

        private RecordWriter<HarshProvisionerContext, TermSet> WriteRecord
        {
            get;
        }
    }
}
