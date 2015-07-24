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

        public List List
        {
            get;
            private set;
        }

        [Obsolete]
        public Boolean ListAdded
        {
            get;
            private set;
        }

        [Parameter(ParameterSetName = "TemplateId")]
        public Int32? TemplateId
        {
            get;
            set;
        }

        [Parameter(ParameterSetName = "TemplateType")]
        public ListTemplateType TemplateType
        {
            get;
            set;
        }

        [Parameter]
        public String Title
        {
            get;
            set;
        }

        [Parameter]
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
                ListAdded = true;
                List = Web.Lists.Add(new ListCreationInformation()
                {
                    TemplateType = TemplateId ?? (Int32)TemplateType,
                    Title = Title,
                    Url = Url,
                });

                await ClientContext.ExecuteQueryAsync();
            }
            else
            {
                List = ExistingList.Value;
            }
        }

        private IResolveSingleOrDefault<List> ExistingList
        {
            get; set;
        }
    }

}
