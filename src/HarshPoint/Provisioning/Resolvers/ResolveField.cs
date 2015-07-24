using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveField :
        ClientObjectResolveBuilder<Field>
    {
        protected override IQueryable<Field>[] CreateQueries(ClientObjectResolveContext context)
        {
            return new[]
            {
                context.ProvisionerContext.Web.Fields,
                context.ProvisionerContext.Web.AvailableFields,
            };
        }

        public ResolveFieldById ById(params Guid[] ids) => new ResolveFieldById(this, ids);
    }
}
