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
            : base(names, StringComparer.Ordinal)
        {
            // uses Ordinal instead of OrdinalIgnoreCase comparer, because there apparently
            // are fields tha`t differ only in case (ParentId vs. ParentID)
        }

        protected override Task<IEnumerable<Field>> ResolveChainElement(HarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveIdentifiers(
                context,
                context.Web.Fields.IncludeWithDefaultProperties(
                    f => f.Id,
                    f => f.InternalName
                ),
                f => f.InternalName
            );
        }
    }
}
