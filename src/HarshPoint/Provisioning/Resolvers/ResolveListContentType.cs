using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListContentType :
        ClientObjectNestedResolveBuilder<ContentType, List>
    {
        public ResolveListContentType(IResolveBuilder<List> parent)
            : base(parent, l => l.ContentTypes)
        {
        }
    }
}
