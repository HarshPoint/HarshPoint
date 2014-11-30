
namespace HarshPoint.Provisioning
{
    public sealed class HarshCompositeProvisioner : HarshProvisioner
    {
        public HarshCompositeProvisioner()
        {
            Provisioners = new HarshProvisionerCollection<HarshProvisioner>(
                p => { p.Context = Context; return p; }
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

        public HarshProvisionerCollection<HarshProvisioner> Provisioners
        {
            get;
            private set;
        }
    }
}
