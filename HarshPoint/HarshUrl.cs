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

        public static String GetRelativeTo(String url, String relativeTo)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(url));
            }

            if (String.IsNullOrWhiteSpace(relativeTo))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(relativeTo));
            }

            if (!relativeTo.EndsWith("/", StringComparison.Ordinal))
            {
                relativeTo += '/';
            }

            if (url.StartsWith(relativeTo, StringComparison.OrdinalIgnoreCase))
            {
                return url.Substring(relativeTo.Length);
            }

            throw Error.ArgumentOutOfRangeFormat(
                nameof(url),
                SR.HarshUrl_UrlNotRelativeTo,
                url,
                relativeTo
            );
        }

        public static async Task<String> EnsureServerRelative(Folder folder, String url)
        {
            if (folder == null)
            {
                throw Error.ArgumentNull(nameof(folder));
            }

            await folder.EnsurePropertyAvailable(s => s.ServerRelativeUrl);
            return EnsureUrlServerRelative(folder.ServerRelativeUrl, url);
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

            var webServerRelativeUrl =
                await web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);

            return EnsureUrlServerRelative(webServerRelativeUrl, url);
        }

        public static String GetLeaf(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(path));
            }

            return path
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
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
