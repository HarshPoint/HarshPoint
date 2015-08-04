using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public static String GetRelativeTo(String url, String relativeTo)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(url));
            }

            if (String.IsNullOrWhiteSpace(relativeTo))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(relativeTo));
            }

            if (!relativeTo.EndsWith("/", StringComparison.Ordinal))
            {
                relativeTo += '/';
            }

            if (url.StartsWith(relativeTo, StringComparison.OrdinalIgnoreCase))
            {
                return url.Substring(relativeTo.Length);
            }

            throw Logger.Fatal.ArgumentFormat(
                nameof(url),
                SR.HarshUrl_UrlNotRelativeTo,
                url,
                relativeTo
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static async Task<String> EnsureServerRelative(Folder folder, String url)
        {
            if (folder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(folder));
            }

            await folder.EnsurePropertyAvailable(s => s.ServerRelativeUrl);
            return EnsureUrlServerRelative(folder.ServerRelativeUrl, url);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static async Task<String> EnsureServerRelative(Site site, String url)
        {
            if (site == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(site));
            }

            await site.EnsurePropertyAvailable(s => s.ServerRelativeUrl);
            return EnsureUrlServerRelative(site.ServerRelativeUrl, url);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static async Task<String> EnsureServerRelative(Web web, String url)
        {
            if (web == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(web));
            }

            var webServerRelativeUrl =
                await web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);

            return EnsureUrlServerRelative(webServerRelativeUrl, url);
        }

        public static String GetLeaf(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(path));
            }

            return path
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();
        }

        public static String GetLeafWithoutExtension(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(path));
            }

            var leaf = GetLeaf(path);
            var lastDot = leaf.LastIndexOf('.');

            if (lastDot > -1)
            {
                return leaf.Substring(0, lastDot);
            }

            return leaf;
        }

        private static String EnsureUrlServerRelative(String rootUrl, String url)
        {
            if (String.IsNullOrWhiteSpace(rootUrl))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(rootUrl));
            }

            if (String.IsNullOrWhiteSpace(url))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(url));
            }

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                return url;
            }

            return Combine(rootUrl, url);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshUrl));
    }
}
