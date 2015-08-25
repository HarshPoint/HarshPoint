using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    [DefaultParameterSet("TemplateType")]
    public class HarshList : HarshProvisioner
    {
        public HarshList()
        {
            ExistingList = DeferredResolveBuilder.Create(
                () => Resolve.List().ByUrl(Url)
            );

            ModifyChildrenContextState(
                () => List
            );

            TemplateType = ListTemplateType.GenericList;
        }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "TemplateId")]
        public Int32? TemplateId
        {
            get;
            set;
        }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "TemplateType")]
        public ListTemplateType TemplateType
        {
            get;
            set;
        }

        [Parameter]
        [MandatoryWhenCreating]
        public String Title
        {
            get;
            set;
        }

        [Parameter(Mandatory = true)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public String Url
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingList.Value.IsNull())
            {
                ValidateMandatoryWhenCreatingParameters();

                List = Web.Lists.Add(new ListCreationInformation()
                {
                    TemplateType = TemplateId ?? (Int32)TemplateType,
                    Title = Title,
                    Url = Url,
                });

                ReportProgress(
                    ProgressReport.Added(Url, List)
                );

                await ClientContext.ExecuteQueryAsync();
            }
            else
            {
                List = ExistingList.Value;

                ReportProgress(
                    ProgressReport.AlreadyExists<List>(Url, List)
                );
            }
        }

        internal IResolveSingleOrDefault<List> ExistingList { get; set; }
        private List List { get; set; }
    }

}
