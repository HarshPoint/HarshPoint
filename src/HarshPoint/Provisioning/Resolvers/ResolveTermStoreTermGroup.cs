using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreTermGroup :
        ClientObjectNestedResolveBuilder<TermGroup, TermStore>
    {
        public ResolveTermStoreTermGroup(
            IResolveBuilder<TermStore> parent
        )
            : base(parent, ts => ts.Groups)
        {
        }
    }
}
