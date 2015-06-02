using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshContentType : HarshProvisioner
    {
        public HarshContentTypeId Id
        {
            get;
            set;
        }

        public String DisplayName
        {
            get;
            set;
        }

        public String Group
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            ContentType = await ResolveAsync(ContentTypeResolver);
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

            if (ContentType.IsNull())
            {
                return;
            }


        }

        private ContentType ContentType
        {
            get;
            set;
        }

        private IResolveSingle<ContentType> ContentTypeResolver
            => Resolve.ContentTypeById(Id);
    }
}
