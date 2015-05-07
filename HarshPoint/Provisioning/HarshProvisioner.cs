using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides context for classes provisioning SharePoint
    /// artifacts using the client-side object model.
    /// </summary>
    public class HarshProvisioner : HarshProvisionerBase<HarshProvisionerContext>, IHarshProvisionerContext
    {
        public ClientContext ClientContext
        {
            get { return Context?.ClientContext; }
        }

        public Site Site
        {
            get { return Context?.Site; }
        }

        public Web Web
        {
            get { return Context?.Web; }
        }

        internal override void ProvisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            ((HarshProvisioner)(p)).Provision(Context);
        }

        internal override void UnprovisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            ((HarshProvisioner)(p)).Unprovision(Context);
        }
    }
}
