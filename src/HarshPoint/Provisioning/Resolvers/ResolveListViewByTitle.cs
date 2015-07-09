using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByTitle : 
        OldNestedResolvable<List, View, String, HarshProvisionerContext, ResolveListViewByTitle>
    {
        public ResolveListViewByTitle(IResolveOld<List> parent, IEnumerable<String> titles) 
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
