using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentType : ClientObjectResolveBuilder<ContentType>
    {
        protected override IQueryable<ContentType>[] CreateQueries(ClientObjectResolveContext context)
        {
            return new[]
            {
                context.ProvisionerContext.Web.ContentTypes,
                context.ProvisionerContext.Web.AvailableContentTypes,
            };
        }

        public ResolveContentTypeById ById(params HarshContentTypeId[] ids)
        {
            return new ResolveContentTypeById(this, ids);
        }

        public ResolveContentTypeById ById(params String[] ids)
        {
            return new ResolveContentTypeById(this, ids.Select(HarshContentTypeId.Parse));
        }
    }
}
