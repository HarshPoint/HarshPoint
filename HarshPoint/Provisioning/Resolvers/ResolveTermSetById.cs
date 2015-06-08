using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermSetById
        : Implementation.NestedResolvable<TermStore, TermSet, Guid, HarshProvisionerContext, ResolveTermSetById>
    {
        public ResolveTermSetById(IResolve<TermStore> termStore, IEnumerable<Guid> ids)
            : base(termStore, ids)
        {
        }

        protected override Task<IEnumerable<TermSet>> ResolveChainElement(HarshProvisionerContext context, TermStore parent)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            return this.ResolveIdentifiers(
                context,
                parent.Groups.SelectMany(group => group.TermSets),
                termSet => termSet.Id
            );
        }
    }
}
