using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshCreateFile : HarshProvisioner
    {
        public File AddedFile
        {
            get;
            private set;
        }

        public IResolveSingle<Folder> Folder
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

        public Boolean OverwriteExisting
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

            var folder = await ResolveSingleAsync(Folder);

            var fci = new FileCreationInformation()
            {
                ContentStream = ContentStream,
                Overwrite = OverwriteExisting,
                Url = await HarshUrl.EnsureServerRelative(folder, FileName),
            };

            AddedFile = folder.Files.Add(fci);
            await folder.Context.ExecuteQueryAsync();
        }
    }
}
