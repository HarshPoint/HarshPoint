using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldByInternalName
        : Implementation.Resolvable<Field, String, HarshProvisionerContext, ResolveFieldByInternalName>
    {
        public ResolveFieldByInternalName(IEnumerable<String> names)
            : base(names, StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override Task<IEnumerable<Field>> ResolveChainElement(HarshProvisionerContext context)
        {
            return this.ResolveIdentifiers(
                context,
                context.Web.Fields.IncludeWithDefaultProperties(f => f.InternalName),
                f => f.InternalName
            );
        }
    }
}
