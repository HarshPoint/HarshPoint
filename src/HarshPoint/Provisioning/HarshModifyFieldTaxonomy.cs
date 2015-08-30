using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Provisioning
{
#warning NOT_TESTED
    public sealed class HarshModifyFieldTaxonomy :
        HarshModifyField<TaxonomyField, HarshModifyFieldTaxonomy>
    {
        public HarshModifyFieldTaxonomy()
        {
            Map(f => f.AllowMultipleValues);
            Map(f => f.IsPathRendered);

            Map(f => f.SspId).From(p => p.TermSet.Value.TermStore.Id);
            Map(f => f.TermSetId).From(p => p.TermSet.Value.Id);
        }

        [Parameter]
        public Boolean? AllowMultipleValues { get; set; }

        [Parameter]
        public Boolean? IsPathRendered { get; set; }

        [Parameter]
        public IResolveSingle<TermSet> TermSet { get; set; }

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<TermSet>(
                ts => ts.TermStore.Id
            );

            base.InitializeResolveContext(context);
        }
    }
}
