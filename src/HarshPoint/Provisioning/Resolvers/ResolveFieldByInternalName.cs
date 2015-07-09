using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldByInternalName :
        Resolvable<Field, String, HarshProvisionerContext, ResolveFieldByInternalName>
    {
        public ResolveFieldByInternalName(IEnumerable<String> names)
            : base(names)
        {
        }

        protected override Task<IEnumerable<Field>> ResolveChainElementOld(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveQuery(
                ClientObjectResolveQuery.FieldByInternalName,
                context,
                context.ProvisionerContext.Web.Fields,
                context.ProvisionerContext.Web.AvailableFields
            );
        }
    }
}
