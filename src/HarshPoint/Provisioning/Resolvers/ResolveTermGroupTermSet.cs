using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermGroupTermSet :
        ClientObjectNestedResolveBuilder<TermSet, TermGroup>
    {
        public ResolveTermGroupTermSet(
            IResolveBuilder<TermGroup> parent
        )
            : base(parent, tg => tg.TermSets)
        {
        }
    }
}
