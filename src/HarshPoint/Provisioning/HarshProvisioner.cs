using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides context for classes provisioning SharePoint
    /// artifacts using the client-side object model.
    /// </summary>
    public class HarshProvisioner : HarshProvisionerBase<HarshProvisionerContext>
    {
        public ClientContext ClientContext => Context?.ClientContext;

        public Site Site => Context?.Site;

        public Web Web => Context?.Web;

        public void ModifyChildrenContextState(Func<ClientObject> modifier)
        {
            ModifyChildrenContextState(() =>
            {
                var result = modifier();

                if (result.IsNull())
                {
                    return null;
                }

                return (Object)(result);
            });
        }

        protected virtual void InitializeResolveContext(ClientObjectResolveContext context)
        {
        }

        protected sealed override ResolveContext<HarshProvisionerContext> CreateResolveContext()
        {
            var ctx = new ClientObjectResolveContext();
            InitializeResolveContext(ctx);
            return ctx;
        }

        internal sealed override async Task OnResolvedParametersBound()
        {
            if (ClientContext.HasPendingRequest)
            {
                await ClientContext.ExecuteQueryAsync();
            }
        }

        internal sealed override Task ProvisionChild(HarshProvisionerBase provisioner, HarshProvisionerContext context)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).ProvisionAsync(context);
        }

        internal sealed override Task UnprovisionChild(HarshProvisionerBase provisioner, HarshProvisionerContext context)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).UnprovisionAsync(context);
        }
    }
}
