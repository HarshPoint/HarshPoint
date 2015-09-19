using HarshPoint.Provisioning.Implementation;
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

            WriteRecord = CreateRecordWriter<List>(() => Url);
        }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "TemplateId")]
        public Int32? TemplateId { get; set; }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "TemplateType")]
        public ListTemplateType TemplateType { get; set; }

        [Parameter]
        [MandatoryWhenCreating]
        public String Title { get; set; }

        [Parameter(Mandatory = true)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public String Url { get; set; }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingList.HasValue)
            {
                List = ExistingList.Value;
                WriteRecord.AlreadyExists(List);
            }
            else
            {
                ValidateMandatoryWhenCreatingParameters();

                List = Web.Lists.Add(new ListCreationInformation()
                {
                    TemplateType = TemplateId ?? (Int32)TemplateType,
                    Title = Title,
                    Url = Url,
                });

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Added(List);
            }
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (ExistingList.HasValue)
            {
                ExistingList.Value.DeleteObject();

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Removed();
            }
            else
            {
                WriteRecord.DidNotExist();
            }
        }

        internal IResolveSingleOrDefault<List> ExistingList { get; set; }
        private List List { get; set; }
        private RecordWriter<HarshProvisionerContext, List> WriteRecord { get; }
    }

}
