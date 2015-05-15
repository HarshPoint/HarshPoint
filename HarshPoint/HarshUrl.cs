using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class HarshUrl
    {
        public static String Combine(params String[] args)
        {
            var parts = args.Select(
                (s, i) => (i == 0) ? s.TrimEnd('/') : s.Trim('/')
            );

            return String.Join("/", parts);
        }
        
        public static async Task<String> EnsureServerRelative(Site site, String url)
        {
            if (site == null)
            {
                throw Error.ArgumentNull(nameof(site));
            }

            await site.EnsurePropertyAvailable(s => s.ServerRelativeUrl);
            return EnsureUrlServerRelative(site.ServerRelativeUrl, url);
        }

        public static async Task<String> EnsureServerRelative(Web web, String url)
        {
            if (web == null)
            {
                throw Error.ArgumentNull(nameof(web));
            }

            await web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);
            return EnsureUrlServerRelative(web.ServerRelativeUrl, url);
        }

        public static String GetLeaf(String url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(url));
            }

            return url
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries )
                .LastOrDefault();
        }

        private static String EnsureUrlServerRelative(String rootUrl, String url)
        {
            if (String.IsNullOrWhiteSpace(rootUrl))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(rootUrl));
            }

            if (String.IsNullOrWhiteSpace(url))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(url));
            }

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                return url;
            }

            return Combine(rootUrl, url);
        }
    }
}
