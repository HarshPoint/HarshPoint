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
            var result = Identifiers.Select(parent.GetTermSet);

            foreach (var termSet in result)
            {
                context.ClientContext.Load(termSet);
            }

            await context.ClientContext.ExecuteQueryAsync();
            return result;
        }
    }
}
