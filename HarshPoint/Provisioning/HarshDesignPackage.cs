using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshDesignPackage : HarshProvisioner
    {
        internal static readonly IResolveSingle<Folder> SolutionCatalogFolder =
            Resolve.Catalog(ListTemplateType.SolutionCatalog).RootFolder();

        public Guid DesignPackageId
        {
            get;
            set;
        }

        public String DesignPackageName
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            CatalogFolder = await TryResolveSingleAsync(SolutionCatalogFolder);
            PackageUrl = await HarshUrl.EnsureServerRelative(CatalogFolder, DesignPackageName);
            PackageInfo = new DesignPackageInfo()
            {
                PackageName = DesignPackageName,
            };
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            DesignPackage.Install(ClientContext, Site, PackageInfo, PackageUrl);
            DesignPackage.Apply(ClientContext, Site, PackageInfo);

            await ClientContext.ExecuteQueryAsync();
            return await base.OnProvisioningAsync();
        }
        
        private Folder CatalogFolder
        {
            get;
            set;
        }

        private DesignPackageInfo PackageInfo
        {
            get;
            set;
        }

        private String PackageUrl
        {
            get;
            set;
        }
    }
}
