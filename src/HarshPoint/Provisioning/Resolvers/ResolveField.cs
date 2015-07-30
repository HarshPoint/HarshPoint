using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveField :
        ClientObjectQueryResolveBuilder<Field>
    {
        protected override IQueryable<Field>[] CreateQueries(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return new[]
            {
                context.ProvisionerContext.Web.Fields,
                context.ProvisionerContext.Web.AvailableFields,
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveField>();
    }
}
