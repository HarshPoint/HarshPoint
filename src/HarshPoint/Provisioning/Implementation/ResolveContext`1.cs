using System;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public class ResolveContext<TProvisionerContext> : ResolveContext
        where TProvisionerContext : IHarshProvisionerContext
    {
        public ResolveContext(TProvisionerContext provisionerContext)
            : base(provisionerContext)
        {
        }

        public new TProvisionerContext ProvisionerContext
            => (TProvisionerContext)base.ProvisionerContext;
    }
}
