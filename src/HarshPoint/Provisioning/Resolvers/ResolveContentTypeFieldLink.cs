using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeFieldLink :
        ClientObjectNestedResolveBuilder<FieldLink, ContentType>
    {
        public ResolveContentTypeFieldLink(IResolveBuilder<ContentType, ClientObjectResolveContext> parent)
            : base(parent, ct => ct.FieldLinks)
        {
        }
    }
}
