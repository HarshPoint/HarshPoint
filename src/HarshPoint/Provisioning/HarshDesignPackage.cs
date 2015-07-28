using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshDesignPackage : HarshProvisioner
    {
        internal static readonly IResolveBuilder<Folder, ClientObjectResolveContext> SolutionCatalogFolder =
            Resolve.Catalog(ListTemplateType.SolutionCatalog).RootFolder();

        public HarshDesignPackage()
        {
            CatalogFolder = SolutionCatalogFolder;
        }

        [Parameter]
        public Guid DesignPackageId
        {
            get;
            set;
        }

        [Parameter]
        public String DesignPackageName
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            PackageUrl = await HarshUrl.EnsureServerRelative(CatalogFolder.Value, DesignPackageName);
            PackageInfo = new DesignPackageInfo()
            {
                PackageName = DesignPackageName,
            };
        }

        protected override async Task OnProvisioningAsync()
        {
            DesignPackage.Install(ClientContext, Site, PackageInfo, PackageUrl);
            DesignPackage.Apply(ClientContext, Site, PackageInfo);

            await ClientContext.ExecuteQueryAsync();
        }
        
        [Parameter]
        private IResolveSingle<Folder> CatalogFolder
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
