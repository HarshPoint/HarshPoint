using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace HarshPoint.Server.Provisioning
{
    internal interface IHarshServerProvisionerContext
    {
        SPWeb Web
        {
            get;
        }

        SPSite Site
        {
            get;
        }

        SPWebApplication WebApplication
        {
            get;
        }

        SPFarm Farm
        {
            get;
        }
    }
}
