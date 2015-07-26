using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Provisioning
{
    public static class ResolveTermStoreExtensions
    {
        public static ResolveTermStoreTermSetById TermSetById(this IResolveOld<TermStore> termStore, params Guid[] ids)
        {
            if (termStore == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(termStore));
            }

            if (ids == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(ids));
            }

            return new ResolveTermStoreTermSetById(termStore, ids);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveTermStoreExtensions));
    }
}
