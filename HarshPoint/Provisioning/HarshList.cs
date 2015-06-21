using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshList : HarshProvisioner
    {
        public HarshList()
        {
            ModifyChildrenContextState(() => List);
            TemplateType = ListTemplateType.GenericList;
        }

        public List List
        {
            get;
            private set;
        }

        public Boolean ListAdded
        {
            get;
            private set;
        }

        public Int32 TemplateId
        {
            get { return (Int32)TemplateType; }
            set { TemplateType = (ListTemplateType)value; }
        }

        public ListTemplateType TemplateType
        {
            get;
            set;
        }

        public String Title
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public String Url
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            List = await TryResolveSingleAsync(ListResolver);
            ListAdded = false;

            await base.InitializeAsync();
        }

        protected override async Task OnProvisioningAsync()
        {
            if (List.IsNull())
            {
                ListAdded = true;
                List = Web.Lists.Add(new ListCreationInformation()
                {
                    TemplateType = TemplateId,
                    Title = Title,
                    Url = Url,
                });

                await ClientContext.ExecuteQueryAsync();
            }
        }

        private IResolve<List> ListResolver => Resolve.ListByUrl(Url);
    }

}
