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
                context.ProvisionerContext.Web.ContentTypes,
                context.ProvisionerContext.Web.AvailableContentTypes,
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveContentType>();
    }
}
