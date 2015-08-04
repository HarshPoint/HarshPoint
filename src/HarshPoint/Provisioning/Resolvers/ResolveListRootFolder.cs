using Microsoft.SharePoint.Client;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListRootFolder : ClientObjectNestedResolveBuilder<Folder, List>
    {
        public ResolveListRootFolder(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent, l => l.RootFolder)
        {
        }
    }
}
