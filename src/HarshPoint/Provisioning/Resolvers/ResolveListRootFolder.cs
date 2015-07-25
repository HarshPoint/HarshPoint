using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListRootFolder : NestedResolveBuilder<Folder, List, ClientObjectResolveContext>
    {
        public ResolveListRootFolder(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent)
        {
        }

        protected override IEnumerable<Folder> SelectChildren(List parent)
            => new[] { parent.RootFolder };
    }
}
