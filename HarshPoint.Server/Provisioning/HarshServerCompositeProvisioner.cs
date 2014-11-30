using HarshPoint.Provisioning;

namespace HarshPoint.Server.Provisioning
{
    public sealed class HarshServerCompositeProvisioner : HarshServerProvisioner
    {
        public HarshServerCompositeProvisioner()
        {
            Provisioners = new HarshProvisionerCollection<HarshProvisionerBase>(
                p => p.ToServerProvisioner(copyContextFrom: this)
            );
        }

        protected override void OnProvisioning()
        {
            Provisioners.Provision();
        }

        protected override void OnUnprovisioning()
        {
            Provisioners.Unprovision();
        }

        public HarshProvisionerCollection<HarshProvisionerBase> Provisioners
        {
            get;
            private set;
        }
    }
}
