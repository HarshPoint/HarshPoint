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

        protected sealed override ResolveContext<HarshProvisionerContext> CreateResolveContext()
        {
            var result = new ClientObjectResolveContextBuilder();
            PrepareResolveContext(result);
            return result.ToResolveContext();
        }

        protected virtual void PrepareResolveContext(ClientObjectResolveContextBuilder context)
        {
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
