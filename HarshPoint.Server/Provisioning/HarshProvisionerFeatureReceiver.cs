using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisionerFeatureReceiver : SPFeatureReceiver
    {
        private readonly Collection<HarshProvisionerBase> _provisioners =
            new Collection<HarshProvisionerBase>();

        public Collection<HarshProvisionerBase> Provisioners
        {
            get { return _provisioners; }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);

            foreach (var provisioner in Provisioners.Select(Setup(properties)))
            {
                provisioner.Provision();
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            foreach (var provisioner in Provisioners.Reverse().Select(Setup(properties)))
            {
                provisioner.Unprovision();
            }

            base.FeatureDeactivating(properties);
        }

        private static Func<HarshProvisionerBase, HarshProvisionerBase> Setup(SPFeatureReceiverProperties properties)
        {
            return provisioner =>
            {
                var clientProvisioner = (provisioner as HarshProvisioner);
                var serverProvisioner = (provisioner as HarshServerProvisioner);

                if (clientProvisioner != null)
                {
                    serverProvisioner = clientProvisioner.ToServerProvisioner();
                }

                if (serverProvisioner != null)
                {
                    serverProvisioner.SetContext(properties.Feature.Parent);
                    return serverProvisioner;
                }

                return provisioner;
            };
        }
    }
}
