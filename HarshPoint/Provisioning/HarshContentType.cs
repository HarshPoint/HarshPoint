using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshContentType : HarshProvisioner
    {
        public String Description
        {
            get;
            set;
        }

        public String Group
        {
            get;
            set;
        }

        public HarshContentTypeId Id
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public IResolveSingle<ContentType> ParentContentType
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
            if (ContentType.IsNull())
            {
                ContentType = Web.ContentTypes.Add(new ContentTypeCreationInformation()
                {
                    Description = Description,
                    Group = Group,
                    Id = Id?.ToString(),
                    ParentContentType = await ParentContentType?.ResolveSingleAsync(Context),
                    Name = Name,
                });

                await ClientContext.ExecuteQueryAsync();
            }

            await base.OnProvisioningAsync();
        }

        protected override HarshProvisionerContext CreateChildrenContext()
        {
            if (!ContentType.IsNull())
            {
                return Context.PushState(ContentType);
            }

            return base.CreateChildrenContext();
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
