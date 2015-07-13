using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    [Obsolete]
    public sealed class OldResolveListByUrl : 
        Resolvable<List, String, HarshProvisionerContext, OldResolveListByUrl>
    {
        public OldResolveListByUrl(IEnumerable<String> urls)
            : base(urls)
        {
        }

        protected override async Task<IEnumerable<List>> ResolveChainElementOld(ResolveContext<HarshProvisionerContext> context)
        {
            return await this.ResolveQuery(
                ClientObjectResolveQuery.ListByUrl,
                context,
                context.ProvisionerContext.Web
            );
        }
    }
}
