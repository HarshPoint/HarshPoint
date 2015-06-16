using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public class ResolveContext<TProvisionerContext> : IResolveContext
        where TProvisionerContext : HarshProvisionerContextBase
    {
        private List<ResolveFailure> _failures;

        public ResolveContext()
        {
        }

        public ResolveContext(TProvisionerContext provisionerContext)
        {
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
            internal set;
        }

        HarshProvisionerContextBase IResolveContext.ProvisionerContext => ProvisionerContext;
    }
}
