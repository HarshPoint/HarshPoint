using HarshPoint.Provisioning;

namespace HarshPoint.Server.Provisioning
{
    public sealed class HarshServerCompositeProvisioner : HarshServerProvisioner
    {
        public HarshServerCompositeProvisioner()
        {
            Provisioners = new HarshProvisionerCollection<HarshProvisionerBase>(p =>
            {
                var serverProvisioner = p.ToServerProvisioner();
                serverProvisioner.CopyContextFrom(this);
                return serverProvisioner;
            });
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
