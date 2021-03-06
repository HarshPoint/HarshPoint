﻿using Microsoft.SharePoint.Client;
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

        [Parameter]
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
            var fci = new FileCreationInformation()
            {
                ContentStream = ContentStream,
                Overwrite = OverwriteExisting,
                Url = await HarshUrl.EnsureServerRelative(Folder.Value, FileName),
            };

            AddedFile = Folder.Value.Files.Add(fci);
            await ClientContext.ExecuteQueryAsync();
        }
    }
}
