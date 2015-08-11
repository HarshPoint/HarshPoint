using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveField : ClientObjectQueryResolveBuilder<Field>
    {
        protected override IQueryable<Field>[] CreateQueries(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return new[]
            {
                // need to make sure at least one include is specified,
                // otherwise CSOM helpfully selects default properties
                // and crashes when running against v15


                context.ProvisionerContext.Web.Fields.Include(ct=>ct.Id),
                context.ProvisionerContext.Web.AvailableFields.Include(ct=>ct.Id),
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveField>();
    }
}
