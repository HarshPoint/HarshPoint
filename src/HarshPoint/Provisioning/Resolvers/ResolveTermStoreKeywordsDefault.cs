using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreKeywordsDefault
        : Implementation.OldResolvable<TermStore, HarshProvisionerContext, ResolveTermStoreKeywordsDefault>
    {
        protected override Task<IEnumerable<TermStore>> ResolveChainElementOld(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Task.FromResult<IEnumerable<TermStore>>(
                ImmutableArray.Create(
                    context.ProvisionerContext.TaxonomySession.GetDefaultKeywordsTermStore()
                )
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermStoreKeywordsDefault>();
    }
}
