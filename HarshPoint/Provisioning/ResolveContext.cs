using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public interface IResolveContext
    {
        HarshProvisionerContextBase ProvisionerContext
        {
            get;
        }

        Boolean ThrowOnFailure
        {
            get;
        }
    }

    public sealed class ResolveContext<TProvisionerContext> : IResolveContext
        where TProvisionerContext : HarshProvisionerContextBase
    {
        internal ResolveContext(TProvisionerContext provisionerContext, Boolean throwOnFailure = false)
        {
            if (provisionerContext == null)
            {
                throw Error.ArgumentNull(nameof(provisionerContext));
            }

            ProvisionerContext = provisionerContext;
            ThrowOnFailure = throwOnFailure;
        }

        public TProvisionerContext ProvisionerContext
        {
            get;
            private set;
        }

        public Boolean ThrowOnFailure
        {
            get;
            private set;
        }

        HarshProvisionerContextBase IResolveContext.ProvisionerContext => ProvisionerContext;
    }
}
