using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreSiteCollectionDefault
        : Implementation.Resolvable<TermStore, HarshProvisionerContext, ResolveTermStoreSiteCollectionDefault>
    {
        protected override Task<IEnumerable<TermStore>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult<IEnumerable<TermStore>>(
                ImmutableArray.Create(
                    context.ProvisionerContext.TaxonomySession.GetDefaultSiteCollectionTermStore()
                )
            );
        }
    }

}
