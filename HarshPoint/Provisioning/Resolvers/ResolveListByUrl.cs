using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListByUrl : ResolveList<String, ResolveListByUrl>
    {
        public ResolveListByUrl(IEnumerable<String> urls)
            : base(urls)
        {
        }

        protected override async Task<IEnumerable<List>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context)
        {
            return await this.ResolveClientObjectQuery(
                context,
                context.ProvisionerContext.Web,
                ClientObjectResolveQuery.ListByUrl
            );
        }
    }
}
