using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Threading.Tasks;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
#warning NOT_TESTED
    public sealed class HarshModifyFieldTaxonomy : HarshModifyField<TaxonomyField>
    {
        [Parameter]
        public Boolean? AllowMultipleValues
        {
            get;
            set;
        }

        [Parameter]
        public Boolean? IsPathRendered
        {
            get;
            set;
        }

        [Parameter]
        public IResolveSingle<TermSet> TermSet
        {
            get;
            set;
        }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
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

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                field.SspId = TermSet.Value.TermStore.Id;
                field.TermSetId = TermSet.Value.Id;

                SetPropertyIfHasValue(field, AllowMultipleValues, f => f.AllowMultipleValues);
                SetPropertyIfHasValue(field, IsPathRendered, f => f.IsPathRendered);

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
