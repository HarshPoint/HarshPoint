using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisionerFeatureReceiver : SPFeatureReceiver
    {
        private readonly HarshServerProvisioner _root = new HarshServerProvisioner();

        public ICollection<HarshProvisionerBase> Provisioners
        {
            get { return _root.Children; }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);

            var context = HarshServerProvisionerContext.FromProperties(properties);
            _root.Provision(context);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var context = HarshServerProvisionerContext.FromProperties(properties);
            _root.Unprovision(context);

            base.FeatureDeactivating(properties);
        }
    }
}
