using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using System.Collections.ObjectModel;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisionerFeatureReceiver : SPFeatureReceiver
    {
        private readonly HarshServerCompositeProvisioner Composite =
            new HarshServerCompositeProvisioner();

        public Collection<HarshProvisionerBase> Provisioners
        {
            get { return Composite.Provisioners; }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);
            Composite.Provision();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            Composite.Unprovision();
            base.FeatureDeactivating(properties);
        }
    }
}
