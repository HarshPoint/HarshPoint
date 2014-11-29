using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Server.Provisioning
{
    public static class HarshProvisionerExtensions
    {
        public static HarshServerProvisioner ToServerProvisioner(this HarshProvisioner provisioner)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull("provisioner");
            }

            return new ClientProvisionerWrapper(provisioner);
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
