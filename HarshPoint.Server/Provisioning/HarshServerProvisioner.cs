using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace HarshPoint.Server.Provisioning
{
    public class HarshServerProvisioner : HarshProvisionerBase<HarshServerProvisionerContext>, IHarshServerProvisionerContext
    {
        public SPWeb Web
        {
            get { return Context?.Web; }
        }

        public SPSite Site
        {
            get { return Context?.Site; }
        }

        public SPWebApplication WebApplication
        {
            get { return Context?.WebApplication; }
        }

        public SPFarm Farm
        {
            get { return Context?.Farm; }
        }

        internal override void ProvisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            p.ToServerProvisioner().Provision(Context);
        }

        internal override void UnprovisionChild(HarshProvisionerBase p)
        {
            if (p == null)
            {
                throw Error.ArgumentNull(nameof(p));
            }

            p.ToServerProvisioner().Unprovision(Context);
        }
    }
}
