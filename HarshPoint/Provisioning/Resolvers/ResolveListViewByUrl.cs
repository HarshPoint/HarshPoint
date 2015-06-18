using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListViewByUrl : 
        NestedResolvable<List, View, String, HarshProvisionerContext, ResolveListViewByUrl>
    {
        public ResolveListViewByUrl(IResolve<List> parent, IEnumerable<String> urls) 
            : base(parent, urls)
        {
        }

        protected override Task<IEnumerable<View>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context, List parent)
        {
            throw new NotImplementedException();
        }
    }
}
