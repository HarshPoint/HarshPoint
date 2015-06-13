using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning
{
    public sealed class ResolveContext<TProvisionerContext> : IResolveContext
        where TProvisionerContext : HarshProvisionerContextBase
    {
        private IImmutableList<ResolveFailure> _failures = ImmutableList<ResolveFailure>.Empty;

        internal ResolveContext(TProvisionerContext provisionerContext, Boolean throwOnFailure = false)
        {
            if (provisionerContext == null)
            {
                throw Error.ArgumentNull(nameof(provisionerContext));
            }

            ProvisionerContext = provisionerContext;
            ThrowOnFailure = throwOnFailure;
        }

        public void AddFailure(Object resolvable, Object identifier)
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (identifier == null)
            {
                throw Error.ArgumentNull(nameof(identifier));
            }

            _failures = _failures.Add(new ResolveFailure(resolvable, identifier));
        }

        public IReadOnlyCollection<ResolveFailure> Failures
            => _failures;

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
