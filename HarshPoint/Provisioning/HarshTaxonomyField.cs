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
            var termSet = await ResolveSingleAsync(TermSet); // TODO: include support on provisioners
            var sspId = await termSet.TermStore.EnsurePropertyAvailable(ts => ts.Id);

            foreach (var field in FieldsResolved)
            {
                field.SspId = sspId;
                field.TermSetId = termSet.Id;

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();

            return await base.OnProvisioningAsync();
        }
    }
}
