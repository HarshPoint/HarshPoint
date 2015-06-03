using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldById
        : Implementation.Resolvable<Field, Guid, HarshProvisionerContext, ResolveFieldById>
    {
        public ResolveFieldById(IEnumerable<Guid> ids)
            : base(ids)
        {
        }

        protected override async Task<IEnumerable<Field>> ResolveChainElement(HarshProvisionerContext context)
        {
            var fields = context.ClientContext.LoadQuery(
                context.Web.Fields.Include(f => f.Id)
            );

            await context.ClientContext.ExecuteQueryAsync();
            return fields.Where(f => Identifiers.Contains(f.Id));
        }
    }
}
