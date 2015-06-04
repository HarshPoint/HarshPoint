using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldByInternalName
        : Implementation.Resolvable<Field, String, HarshProvisionerContext, ResolveFieldByInternalName>
    {
        public ResolveFieldByInternalName(IEnumerable<String> names)
            : base(names)
        {
        }

        protected override async Task<IEnumerable<Field>> ResolveChainElement(HarshProvisionerContext context)
        {
            var fields = context.ClientContext.LoadQuery(
                context.Web.Fields.Include(
                    f => f.Id,
                    f => f.InternalName
                )
            );

            await context.ClientContext.ExecuteQueryAsync();
            return fields.Where(f => Identifiers.Contains(f.InternalName));
        }
    }
}
