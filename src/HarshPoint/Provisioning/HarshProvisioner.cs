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

        protected new ClientObjectManualResolver ManualResolver
            => (ClientObjectManualResolver)base.ManualResolver;

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

        internal sealed override ManualResolver CreateManualResolver(Func<IResolveContext> resolveContextFactory)
            => new ClientObjectManualResolver(
                () => (ClientObjectResolveContext)resolveContextFactory()
            );

        protected sealed override ResolveContext<HarshProvisionerContext> CreateResolveContext()
        {
            var ctx = new ClientObjectResolveContext(Context);
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
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).ProvisionAsync(context);
        }

        internal sealed override Task UnprovisionChild(HarshProvisionerBase provisioner, HarshProvisionerContext context)
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).UnprovisionAsync(context);
        }
    }
}
