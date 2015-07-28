using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreSiteCollectionDefault :
        ResolveBuilder<TermStore, ClientObjectResolveContext>
    {
        protected override Object Initialize(ClientObjectResolveContext context)
            => null;

        protected override IEnumerable ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ImmutableArray.Create(
                context.ProvisionerContext.TaxonomySession.GetDefaultSiteCollectionTermStore()
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermStoreSiteCollectionDefault>();
    }
}
