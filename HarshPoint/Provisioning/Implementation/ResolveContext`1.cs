using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public sealed class ResolveContext<TProvisionerContext> : IResolveContext
        where TProvisionerContext : HarshProvisionerContextBase
    {
        private List<ResolveFailure> _failures;

        internal ResolveContext(TProvisionerContext provisionerContext)
        {
            if (provisionerContext == null)
            {
                throw Error.ArgumentNull(nameof(provisionerContext));
            }

            ProvisionerContext = provisionerContext;
        }

        public void AddFailure(Object resolvable, Object identifier)
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }


            if (_failures == null)
            {
                _failures = new List<ResolveFailure>();
            }

            _failures.Add(new ResolveFailure(resolvable, identifier));
        }

        public void ValidateNoFailures()
        {
            if (_failures != null && _failures.Any())
            {
                throw new ResolveFailedException(_failures);
            }
        }

        public IReadOnlyCollection<ResolveFailure> Failures
            => _failures;

        public TProvisionerContext ProvisionerContext
        {
            get;
            private set;
        }

        HarshProvisionerContextBase IResolveContext.ProvisionerContext => ProvisionerContext;
    }
}
