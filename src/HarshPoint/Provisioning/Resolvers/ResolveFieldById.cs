using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldById
        : Resolvable<Field, Guid, HarshProvisionerContext, ResolveFieldById>
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

            return this.ResolveQuery(
                ClientObjectResolveQuery.FieldById,
                context,
                context.ProvisionerContext.Web.Fields,
                context.ProvisionerContext.Web.AvailableFields
            );
        }
    }
}
