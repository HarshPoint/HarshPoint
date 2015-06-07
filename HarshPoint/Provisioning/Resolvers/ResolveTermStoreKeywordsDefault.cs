using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreKeywordsDefault
        : Implementation.Resolvable<TermStore, HarshProvisionerContext, ResolveTermStoreKeywordsDefault>
    {
        protected override Task<IEnumerable<TermStore>> ResolveChainElement(HarshProvisionerContext context)
        {
            return Task.FromResult(
                HarshSingleElementCollection.Create(
                    context.TaxonomySession.GetDefaultKeywordsTermStore()
                )
            );
        }
    }

}
