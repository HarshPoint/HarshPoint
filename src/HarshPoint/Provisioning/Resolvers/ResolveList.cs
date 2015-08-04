using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveList : ClientObjectQueryResolveBuilder<List>
    {
        protected override IQueryable<List> CreateQuery(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return context.ProvisionerContext.Web.Lists;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveList>();
    }
}
