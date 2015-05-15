using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface IHarshProvisionerContext
    {
        ClientContext ClientContext
        {
            get;
        }

        Site Site
        {
            get;
        }

        Web Web
        {
            get;
        }
    }
}
