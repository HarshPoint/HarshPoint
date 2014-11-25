using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisionerFeatureReceiver : SPFeatureReceiver
    {
        private readonly Collection<HarshProvisioner> _provisioners =
            new Collection<HarshProvisioner>();

        public Collection<HarshProvisioner> Provisioners
        {
            get { return _provisioners; }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);

            foreach (var provisioner in Provisioners)
            {
                SetupProvisioner(provisioner, properties.Feature.Parent);
                provisioner.Provision();
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            foreach (var provisioner in Provisioners.Reverse())
            {
                SetupProvisioner(provisioner, properties.Feature.Parent);
                provisioner.Unprovision();
            }

            base.FeatureDeactivating(properties);
        }

        private void SetupProvisioner(HarshProvisioner provisioner, Object parent)
        {
            provisioner.Web = (parent as SPWeb);
            provisioner.Site = (parent as SPSite);
            provisioner.WebApplication = (parent as SPWebApplication);
            provisioner.Farm = (parent as SPFarm);
        }
    }
}
