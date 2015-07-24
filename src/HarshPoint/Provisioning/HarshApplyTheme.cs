using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshApplyTheme : HarshProvisioner
    {
        public String BackgroundImageUrl
        {
            get;
            set;
        }

        public String ColorPaletteUrl
        {
            get;
            set;
        }

        public String FontSchemeUrl
        {
            get;
            set;
        }

        public Boolean ShareGenerated
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            await Web.EnsurePropertyAvailable(w => w.ServerRelativeUrl);

            var backgroundImageUrl = await EnsureServerRelativeOrNull(BackgroundImageUrl);
            var colorPaletteUrl = await EnsureServerRelativeOrNull(ColorPaletteUrl);
            var fontSchemeUrl = await EnsureServerRelativeOrNull(FontSchemeUrl);

            Web.ApplyTheme(
                colorPaletteUrl, 
                fontSchemeUrl, 
                backgroundImageUrl, 
                ShareGenerated
            );

            await ClientContext.ExecuteQueryAsync();
        }

        private async Task<String> EnsureServerRelativeOrNull(String url)
        {
            if (url == null)
            {
                return null;
            }

            return await HarshUrl.EnsureServerRelative(Site, url);
        }
    }
}