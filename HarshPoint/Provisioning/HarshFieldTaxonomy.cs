using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshFieldTaxonomy : HarshFieldProvisioner<TaxonomyField>
    {
        public Boolean? AllowMultipleValues
        {
            get;
            set;
        }

        public Boolean? IsPathRendered
        {
            get;
            set;
        }

        public IResolve<TermSet> TermSet
        {
            get;
            set;
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var termSet = await ResolveSingleAsync(
                TermSet.Include(ts => ts.TermStore.Id)
            );

            foreach (var field in FieldsResolved)
            {
                field.SspId = termSet.TermStore.Id;
                field.TermSetId = termSet.Id;

                SetPropertyIfHasValue(field, AllowMultipleValues, f => f.AllowMultipleValues);
                SetPropertyIfHasValue(field, IsPathRendered, f => f.IsPathRendered);

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();

            return await base.OnProvisioningAsync();
        }
    }
}
