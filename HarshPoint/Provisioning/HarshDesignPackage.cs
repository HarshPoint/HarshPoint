using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshDesignPackage : HarshProvisioner
    {
        public String DesignPackageUrl
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

            var wsp = new DesignPackageInfo()
            {
                PackageName = HarshUrl.GetLeaf(DesignPackageUrl),
            };

            var serverRelativeUrl = await HarshUrl.EnsureServerRelative(Site, DesignPackageUrl);

            Microsoft.SharePoint.Client.Publishing.DesignPackage.Install(ClientContext, Site, wsp, serverRelativeUrl);

            await ClientContext.ExecuteQueryAsync();

        }
    }
}
