using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreKeywordsDefault :
        ClientObjectResolveBuilder<TermStore>
    {
        protected override IEnumerable<TermStore> CreateObjects(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ImmutableArray.Create(
                context.ProvisionerContext.TaxonomySession.GetDefaultKeywordsTermStore()
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermStoreKeywordsDefault>();
    }
}
