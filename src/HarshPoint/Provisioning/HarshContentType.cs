using HarshPoint.Provisioning.Implementation;
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

        [Parameter]
        public String Description { get; set; }

        [Parameter]
        [DefaultFromContext(typeof(DefaultContentTypeGroup))]
        public String Group { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Id")]
        public HarshContentTypeId Id { get; set; }

        [Parameter(Mandatory = false /*WhenCreating*/)]
        public String Name { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ParentContentType")]
        public IResolveSingle<ContentType> ParentContentType { get; set; }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.StringId
            );

            base.InitializeResolveContext(context);
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

                ReportProgress(
                    ProgressReport.Added(
                        Id?.ToString() ?? Name,
                        ContentType
                    )
                );

                await ClientContext.ExecuteQueryAsync();
            }
            else
            {
                ContentType = ExistingContentType.Value;

                ReportProgress(
                    ProgressReport.AlreadyExists(ContentType.StringId, ContentType)
                );
            }
        }


        private ContentType ContentType { get; set; }
        internal IResolveSingleOrDefault<ContentType> ExistingContentType { get; set; }
    }
}
