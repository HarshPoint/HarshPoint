using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListRootFolder : Implementation.OldNestedResolvable<List, Folder, HarshProvisionerContext, ResolveListRootFolder>
    {
        public ResolveListRootFolder(IResolveOld<List> lists)
            : base(lists)
        {
        }

        protected override Task<IEnumerable<Folder>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context, List parent)
        {
            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            return Task.FromResult<IEnumerable<Folder>>(
                ImmutableArray.Create(parent.RootFolder)
            );
        }
    }
}
