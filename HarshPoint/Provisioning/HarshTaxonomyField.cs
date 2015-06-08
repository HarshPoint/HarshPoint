using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshTaxonomyField : HarshFieldProvisioner<TaxonomyField>
    {
        public IResolveSingle<TermSet> TermSet
        {
            get;
            set;
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var termSet = await ResolveSingleAsync(TermSet);

            foreach (var field in FieldsResolved)
            {
                field.SspId = termSet.TermStore.Id;
                field.TermSetId = termSet.Id;

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();

            return await base.OnProvisioningAsync();
        }
    }
}
