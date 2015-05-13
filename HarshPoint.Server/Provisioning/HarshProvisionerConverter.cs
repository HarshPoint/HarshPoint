using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Server.Provisioning
{
    public static class HarshProvisionerConverter
    {
        public static HarshServerProvisioner ToServerProvisioner(this HarshProvisionerBase provisioner)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
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
                    nameof(provisioner),
                    SR.HarshServerProvisionerConverter_CannotConvert,
                    provisioner.GetType().FullName
                );
            }

            return serverProvisioner;
        }

        private sealed class ClientProvisionerWrapper : HarshServerProvisioner
        {
            public ClientProvisionerWrapper(HarshProvisioner provisioner)
            {
                Provisioner = provisioner;
            }

            private ClientContext ClientContext
            {
                get;
                set;
            }

            private HarshProvisionerContext ProvisionerContext
            {
                get;
                set;
            }

            private HarshProvisioner Provisioner
            {
                get;
                set;
            }

            protected override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                if (Web == null)
                {
                    throw Error.InvalidOperation(SR.HarshServerProvisionerConverter_OnlyWebAndSiteSupported);
                }

                ClientContext = new ClientContext(Web.Url);
                ProvisionerContext = new HarshProvisionerContext(ClientContext);
            }

            protected override void Complete()
            {
                ProvisionerContext = null;

                if (ClientContext != null)
                {
                    ClientContext.Dispose();
                    ClientContext = null;
                }

                base.Complete();
            }

            internal override ICollection<HarshProvisionerBase> CreateChildrenCollection()
            {
                return NoChildren;
            }

            protected override Task OnProvisioningAsync()
            {
                return Provisioner.ProvisionAsync(ProvisionerContext);
            }

            protected override Task OnUnprovisioningAsync()
            {
                return Provisioner.UnprovisionAsync(ProvisionerContext);
            }
        }
    }
}
