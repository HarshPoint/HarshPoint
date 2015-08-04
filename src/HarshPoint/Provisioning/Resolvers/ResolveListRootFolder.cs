using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListRootFolder : NestedResolveBuilder<Folder, List, ClientObjectResolveContext>
    {
        public ResolveListRootFolder(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent)
        {
        }

        protected override IEnumerable<Folder> SelectChildren(List parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            return new[] { parent.RootFolder };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListRootFolder>();
    }
}
