using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisionerFeatureReceiver : SPFeatureReceiver
    {
        private readonly HarshServerProvisioner _root = new RootProvisioner();

        public ICollection<HarshProvisionerBase> Provisioners => _root.Children;

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);

            var context = HarshServerProvisionerContext.FromProperties(properties);
            _root.ProvisionAsync(context).RunSynchronously();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var context = HarshServerProvisionerContext.FromProperties(properties);
            _root.UnprovisionAsync(context).RunSynchronously();

            base.FeatureDeactivating(properties);
        }

        public override void FeatureUpgrading(
            SPFeatureReceiverProperties properties, 
            String upgradeActionName, 
            IDictionary<String, String> parameters)
        {
            base.FeatureUpgrading(properties, upgradeActionName, parameters);

            var context = HarshServerProvisionerContext.FromProperties(
                properties, 
                upgradeActionName, 
                parameters
            );
            _root.ProvisionAsync(context).RunSynchronously();
        }

        private sealed class RootProvisioner : HarshServerProvisioner
        {
            protected override Boolean ShouldProvisionChild(HarshServerProvisioner provisioner)
            {
                if (provisioner == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(provisioner));
                }

                if (UpgradeAction == null)
                {
                    return base.ShouldProvisionChild(provisioner);
                }

                var runOnUpgradeActions = new HashSet<String>(StringComparer.Ordinal);
                provisioner.AddRunOnUpgradeActionsTo(runOnUpgradeActions);

                return runOnUpgradeActions.Contains(UpgradeAction);
            }
        }
    }
}
