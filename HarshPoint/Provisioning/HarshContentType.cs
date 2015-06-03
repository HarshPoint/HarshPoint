using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshContentType : HarshProvisioner
    {
        public ContentType ContentType
        {
            get;
            private set;
        }

        public Boolean ContentTypeAdded
        {
            get;
            private set;
        }

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
            if ((Id != null) && (ParentContentType != null))
            {
                throw Error.InvalidOperation(
                    SR.HarshContentType_BothIdAndParentContentTypeCannotBeSet
                );
            }

            if (Id != null)
            {
                ContentType = await ResolveSingleOrDefaultAsync(Resolve.ContentTypeById(Id));
            }
            else
            {
                throw Error.InvalidOperation("TODO: Should lookup by name.");
            }
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ContentType.IsNull())
            {
                ContentType parentContentType = null;

                if (ParentContentType != null)
                {
                    parentContentType = await ResolveSingleOrDefaultAsync(ParentContentType);
                }

                ContentType = Web.ContentTypes.Add(new ContentTypeCreationInformation()
                {
                    Description = Description,
                    Group = Group,
                    Id = Id?.ToString(),
                    ParentContentType = parentContentType,
                    Name = Name,
                });

                await ClientContext.ExecuteQueryAsync();

                ContentTypeAdded = true;
            }
            else
            {
                ContentTypeAdded = false;
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

    }
}
