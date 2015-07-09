using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByUrl : 
        OldNestedResolvable<List, View, String, HarshProvisionerContext, ResolveListViewByUrl>
    {
        public ResolveListViewByUrl(IResolveOld<List> parent, IEnumerable<String> urls) 
            : base(parent, urls)
        {
        }

        protected override Task<IEnumerable<View>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context, List parent)
        {
            return this.ResolveQuery(
                ClientObjectResolveQuery.ListViewByUrl,
                context,
                parent
            );
        }
    }
}
