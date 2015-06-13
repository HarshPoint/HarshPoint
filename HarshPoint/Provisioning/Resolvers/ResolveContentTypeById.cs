using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeById :
        Implementation.Resolvable<ContentType, HarshContentTypeId, HarshProvisionerContext, ResolveContentTypeById>
    {
        public ResolveContentTypeById(IEnumerable<HarshContentTypeId> identifiers)
            : base(identifiers)
        {
        }

        protected override Task<IEnumerable<ContentType>> ResolveChainElement(HarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return this.ResolveClientObjectQuery(
                context,
                context.Web.ContentTypes,
                ClientObjectResolveQuery.ContentTypeById
            );
        }
    }
}