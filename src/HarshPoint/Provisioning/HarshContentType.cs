using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshContentType : HarshProvisioner
    {
        public HarshContentType()
        {
            ModifyChildrenContextState(
                () => ContentType
            );

            ExistingContentType = DeferredResolveBuilder.Create(
                () => Resolve.ContentType().ById(Id)
            );
        }

        public ContentType ContentType
        {
            get;
            private set;
        }

        [Parameter]
        public String Description
        {
            get;
            set;
        }

        [Parameter]
        [DefaultFromContext(typeof(DefaultContentTypeGroup))]
        public String Group
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ParameterSetName = "Id")]
        public HarshContentTypeId Id
        {
            get;
            set;
        }

        [Parameter(Mandatory = true)]
        public String Name
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ParameterSetName = "ParentContentType")]
        public IResolveSingle<ContentType> ParentContentType
        {
            get;
            set;
        }
        
        protected override async Task OnProvisioningAsync()
        {
            if (ExistingContentType.Value == null)
            {
                ContentType = Web.ContentTypes.Add(new ContentTypeCreationInformation()
                {
                    Description = Description,
                    Group = Group,
                    Id = Id?.ToString(),
                    ParentContentType = ParentContentType?.Value,
                    Name = Name,
                });

                await ClientContext.ExecuteQueryAsync();
            }
        }

        [Parameter]
        private IResolveSingleOrDefault<ContentType> ExistingContentType
        {
            get;
            set;
        }
    }
}
