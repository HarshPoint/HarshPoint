using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
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

        protected override Task<IEnumerable<Field>> ResolveChainElement(HarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveIdentifiers(
                context,
                context.Web.Fields.Include(f => f.Id),
                f => f.Id
            );
        }
    }
}
