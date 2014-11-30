using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public sealed class HarshCompositeProvisioner : HarshProvisioner
    {
        public HarshCompositeProvisioner()
        {
            Provisioners = new Collection<HarshProvisioner>();
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();

            foreach (var provisioner in Provisioners)
            {
                provisioner.Context = Context;
                provisioner.Provision();
            }
        }

        protected override void OnUnprovisioning()
        {
            foreach (var provisioner in Provisioners.Reverse())
            {
                provisioner.Context = Context;
                provisioner.Unprovision();
            }

            base.OnUnprovisioning();
        }

        public Collection<HarshProvisioner> Provisioners
        {
            get;
            private set;
        }
    }
}
