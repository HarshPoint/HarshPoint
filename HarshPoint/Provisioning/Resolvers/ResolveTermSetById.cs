using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermSetById
        : Implementation.Resolvable<TermSet, Guid, HarshProvisionerContext, ResolveTermSetById>
    {
        private Func<TaxonomySession, TermStore> _termStore = TermStoreDefaultSiteCollection;

        public ResolveTermSetById(IEnumerable<Guid> ids)
            : base(ids)
        {
        }

        public ResolveTermSetById InDefaultKeywordStore()
        {
            return this.With(r => r._termStore, TermStoreDefaultSiteCollection);
        }

        protected override Task<IEnumerable<TermSet>> ResolveChainElement(HarshProvisionerContext context)
        {
            throw new NotImplementedException();
        }

        private static readonly Func<TaxonomySession, TermStore> TermStoreDefaultKeywords =
            session => session.GetDefaultKeywordsTermStore();

        private static readonly Func<TaxonomySession, TermStore> TermStoreDefaultSiteCollection =
            session => session.GetDefaultSiteCollectionTermStore();
    }
}
