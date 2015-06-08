using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListByUrl : ResolveList<String, ResolveListByUrl>
    {
        public ResolveListByUrl(IEnumerable<String> urls)
            : base(urls, StringComparer.OrdinalIgnoreCase) // TODO: replace with slash-tolerant comparer
        {
        }

        protected override async Task<IEnumerable<List>> ResolveChainElement(HarshProvisionerContext context)
        {
            var webServerRelativeUrl = await context.Web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);

            return await this.ResolveIdentifiers(
                context,
                context.Web.Lists.Include(l => l.RootFolder.ServerRelativeUrl),
                l => HarshUrl.GetRelativeTo(l.RootFolder.ServerRelativeUrl, webServerRelativeUrl)
            );
        }
    }
}
