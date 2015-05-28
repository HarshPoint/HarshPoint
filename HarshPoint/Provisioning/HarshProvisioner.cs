using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides context for classes provisioning SharePoint
    /// artifacts using the client-side object model.
    /// </summary>
    public class HarshProvisioner : HarshProvisionerBase<HarshProvisionerContext>
    {
        public ClientContext ClientContext => Context?.ClientContext;

        public Site Site => Context?.Site;

        public Web Web => Context?.Web;

        internal override Task ProvisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            return ((HarshProvisioner)(p)).ProvisionAsync(Context);
        }

        internal override Task UnprovisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            return ((HarshProvisioner)(p)).UnprovisionAsync(Context);
        }
    }
}
