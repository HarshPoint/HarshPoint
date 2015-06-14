using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeById :
        Resolvable<ContentType, HarshContentTypeId, HarshProvisionerContext, ResolveContentTypeById>
    {
        public ResolveContentTypeById(IEnumerable<HarshContentTypeId> identifiers)
            : base(identifiers)
        {
        }

        protected override Task<IEnumerable<ContentType>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveQuery(
                ClientObjectResolveQuery.ContentTypeById,
                context,
                context.ProvisionerContext.Web.ContentTypes
            );
        }
    }
}