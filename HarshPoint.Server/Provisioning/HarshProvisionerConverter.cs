using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Server.Provisioning
{
    public static class HarshProvisionerConverter
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
                    SR.HarshServerProvisionerConverter_CannotConvert,
                    provisioner.GetType().FullName
                );
            }

            return serverProvisioner;
        }

        internal static HarshServerProvisioner ToServerProvisioner(this HarshProvisionerBase provisioner, HarshServerProvisioner copyContextFrom)
        {
            var serverProvisioner = ToServerProvisioner(provisioner);

            if (copyContextFrom != null)
            {
                serverProvisioner.CopyContextFrom(copyContextFrom);
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
                    throw Error.InvalidOperation(SR.HarshServerProvisionerConverter_OnlyWebAndSiteSupported);
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
