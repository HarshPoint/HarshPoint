using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class WebExtensions
    {
        public static async Task<List> GetListByWebRelativeUrl(this Web web, String url)
        {
            if (web == null)
            {
                throw Error.ArgumentNull(nameof(web));
            }

            if (url == null)
            {
                throw Error.ArgumentNull(nameof(url));
            }

            var serverRelativeUrl = await HarshUrl.EnsureServerRelative(web, url);
            return web.GetList(serverRelativeUrl);
        }
    }
}
