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

            // need to make sure at least one include is specified,
            // otherwise CSOM helpfully selects default properties
            // and crashes when running against v15

            return context.ProvisionerContext.Web.Lists.Include(l => l.Id);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveList>();
    }
}
