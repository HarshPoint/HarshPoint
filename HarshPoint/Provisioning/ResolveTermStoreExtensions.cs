using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Provisioning
{
    public static class ResolveTermStoreExtensions
    {
        public static ResolveTermSetById TermSetById(this IResolve<TermStore> termStore, params Guid[] ids)
        {
            if (termStore == null)
            {
                throw Error.ArgumentNull(nameof(termStore));
            }

            if (ids == null)
            {
                throw Error.ArgumentNull(nameof(ids));
            }

            return new ResolveTermSetById(termStore, ids);
        }
    }
}
