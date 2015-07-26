using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreSiteCollectionDefault
        : Implementation.OldResolvable<TermStore, HarshProvisionerContext, ResolveTermStoreSiteCollectionDefault>
    {
        protected override Task<IEnumerable<TermStore>> ResolveChainElementOld(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Task.FromResult<IEnumerable<TermStore>>(
                ImmutableArray.Create(
                    context.ProvisionerContext.TaxonomySession.GetDefaultSiteCollectionTermStore()
                )
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermStoreSiteCollectionDefault>();
    }
}
