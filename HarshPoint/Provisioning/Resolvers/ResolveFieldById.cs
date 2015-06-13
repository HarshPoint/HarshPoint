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

        protected override Task<IEnumerable<Field>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveClientObjectQuery(
                context,
                context.ProvisionerContext.Web.Fields,
                ClientObjectResolveQuery.FieldById
            );
        }
    }
}
