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

        protected override async Task<IEnumerable<List>> ResolveChainElement(HarshProvisionerContext context)
        {
            await context.Web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);

            return await this.ResolveClientObjectQuery(
                context,
                context.Web,
                ClientObjectResolveQuery.ListByUrl
            );
        }
    }
}
