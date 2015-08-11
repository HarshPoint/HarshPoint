using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentType : ClientObjectQueryResolveBuilder<ContentType>
    {
        protected override IQueryable<ContentType>[] CreateQueries(ClientObjectResolveContext context)
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

                context.ProvisionerContext.Web.ContentTypes.Include(ct=>ct.StringId),
                context.ProvisionerContext.Web.AvailableContentTypes.Include(ct=>ct.StringId),
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveContentType>();
    }
}
