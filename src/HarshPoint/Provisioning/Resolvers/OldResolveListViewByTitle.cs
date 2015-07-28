using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class OldResolveListViewByTitle : 
        OldNestedResolvable<List, View, String, HarshProvisionerContext, OldResolveListViewByTitle>
    {
        public OldResolveListViewByTitle(IResolveOld<List> parent, IEnumerable<String> titles) 
            : base(parent, titles)
        {
        }

        protected override Task<IEnumerable<View>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context, List parent)
        {
            return this.ResolveQuery(
                ClientObjectResolveQuery.ListViewByTitle,
                context,
                parent
            );
        }
    }
}
