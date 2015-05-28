using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshSiteGroup : HarshProvisioner
    {
        public String Description
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            var groups = ClientContext.LoadQuery(Web.SiteGroups);
            await ClientContext.ExecuteQueryAsync();

            Group = groups.FirstOrDefaultByProperty(
                x => x.Title, 
                Name, 
                StringComparer.OrdinalIgnoreCase
            );

            await base.InitializeAsync();
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

            if (Group.IsNull())
            {
                Group = Web.SiteGroups.Add(new GroupCreationInformation()
                {
                    Title = Name,
                    Description = Description,
                });

                await ClientContext.ExecuteQueryAsync();
            }
        }

        private Group Group
        {
            get;
            set;
        }
    }
}
