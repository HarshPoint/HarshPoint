using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListView : ClientObjectNestedResolveBuilder<View, List>
    {
        public ResolveListView(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent, l => l.Views)
        {
        }
    }
}
