using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HarshPoint.Server.Provisioning
{
    public class HarshServerProvisioner : HarshProvisionerBase<HarshServerProvisionerContext>
    {
        private HashSet<String> _runOnUpgradeActions;

        public HarshServerProvisioner RunOnUpgradeAction(String action)
        {
            if (String.IsNullOrWhiteSpace(action))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(action));
            }

            if (_runOnUpgradeActions == null)
            {
                _runOnUpgradeActions = new HashSet<String>(StringComparer.Ordinal);
            }

            _runOnUpgradeActions.Add(action);
            return this;
        }

        public SPFarm Farm => Context?.Farm;

        public SPSite Site => Context?.Site;

        public String UpgradeAction => Context?.UpgradeAction;

        public IReadOnlyDictionary<String, String> UpgradeArguments => Context?.UpgradeArguments;

        public SPWeb Web => Context?.Web;

        public SPWebApplication WebApplication => Context?.WebApplication;

        protected virtual Boolean ShouldProvisionChild(HarshServerProvisioner provisioner) => true;

        protected virtual Boolean ShouldUnprovisionChild(HarshServerProvisioner provisioner) => true;

        internal void AddRunOnUpgradeActionsTo(ICollection<String> runOnUpgradeActions)
        {
            if (runOnUpgradeActions == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(runOnUpgradeActions));
            }

            if (_runOnUpgradeActions != null)
            {
                runOnUpgradeActions.AddRange(_runOnUpgradeActions);
            }

            if (!HasChildren)
            {
                return;
            }

            foreach (var child in Children.Select(HarshProvisionerConverter.ToServerProvisioner))
            {
                child.AddRunOnUpgradeActionsTo(runOnUpgradeActions);
            }
        }

        protected sealed override async Task ProvisionChild(
            HarshProvisionerBase provisioner, 
            HarshServerProvisionerContext context,
            CancellationToken token
        )
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            var serverProvisioner = provisioner.ToServerProvisioner();

            if (ShouldProvisionChild(serverProvisioner))
            {
                await serverProvisioner.ProvisionAsync(context, token);
            }
        }

        protected sealed override async Task UnprovisionChild(
            HarshProvisionerBase provisioner, 
            HarshServerProvisionerContext context,
            CancellationToken token
        )
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            var serverProvisioner = provisioner.ToServerProvisioner();

            if (ShouldUnprovisionChild(serverProvisioner))
            {
                await serverProvisioner.UnprovisionAsync(context, token);
            }
        }
    }
}
