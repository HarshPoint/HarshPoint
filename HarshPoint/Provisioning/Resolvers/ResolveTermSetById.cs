using Microsoft.SharePoint.Client;
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

        protected override async Task<IEnumerable<TermSet>> ResolveChainElement(HarshProvisionerContext context, TermStore parent)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            var groups = context.ClientContext.LoadQuery(
                parent.Groups.Include(
                    g => g.TermSets.Include(
                        ts => ts.Id
                    )
                )
            );
            await context.ClientContext.ExecuteQueryAsync();

            return this.ResolveIdentifiers(
                context,
                groups.SelectMany(g => g.TermSets),
                ts => ts.Id
            );
        }
    }
}
