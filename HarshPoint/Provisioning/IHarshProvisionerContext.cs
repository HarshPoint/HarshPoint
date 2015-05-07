using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning
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
