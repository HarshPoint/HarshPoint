using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Server.Provisioning
{
    public static class HarshProvisionerExtensions
    {
        public static HarshServerProvisioner ToServerProvisioner(this HarshProvisionerBase provisioner)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull("provisioner");
            }

            var clientProvisioner = (provisioner as HarshProvisioner);
            var serverProvisioner = (provisioner as HarshServerProvisioner);

            if (clientProvisioner != null)
            {
                serverProvisioner = new ClientProvisionerWrapper(clientProvisioner);
            }

            if (serverProvisioner == null)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "provisioner",
                    provisioner,
                    SR.HarshServerProvisionerExtensions_CannotConvertTo,
                    provisioner.GetType().FullName
                );
            }

            return serverProvisioner;
        }

        private sealed class ClientProvisionerWrapper : HarshServerProvisioner
        {
            private readonly HarshProvisioner _provisioner;

            public ClientProvisionerWrapper(HarshProvisioner provisioner)
            {
                _provisioner = provisioner;
            }

            protected override void Initialize()
            {
                if (Web == null)
                {
                    throw Error.InvalidOperation(SR.HarshServerProvisionerExtensions_OnlyWebAndSiteSupported);
                }

                _provisioner.Context = new ClientContext(Web.Url);
            }

            protected override void OnProvisioning()
            {
                _provisioner.Provision();
            }

            protected override void OnUnprovisioning()
            {
                _provisioner.Unprovision();
            }
        }
    }
}
