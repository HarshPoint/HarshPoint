using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HarshPoint.Provisioners
{
    public sealed class HarshCreateFile : HarshProvisioner
    {
        public File AddedFile
        {
            get;
            private set;
        }

        public ListTemplateType Catalog
        {
            get;
            set;
        }

        public Stream ContentStream
        {
            get;
            set;
        }

        public String FileName
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

            var catalog = Web.GetCatalog((Int32)(Catalog));
            var folder = catalog.RootFolder;

            await folder.EnsurePropertyAvailable(f => f.ServerRelativeUrl);

            var fci = new FileCreationInformation()
            {
                ContentStream = ContentStream,
                Url = HarshUrl.Combine(folder.ServerRelativeUrl, FileName),
            };

            AddedFile = folder.Files.Add(fci);
            await folder.Context.ExecuteQueryAsync();
        }
    }
}
