using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning
{
    public abstract class ResolveContext
    {
        private List<ResolveFailure> _failures;

        protected ResolveContext(IHarshProvisionerContext provisionerContext)
        {
            if (provisionerContext == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisionerContext));
            }

            ProvisionerContext = provisionerContext;
        }

        public void AddFailure(IResolveBuilder resolveBuilder, Object identifier)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }


            if (_failures == null)
            {
                _failures = new List<ResolveFailure>();
            }

            _failures.Add(new ResolveFailure(resolveBuilder, identifier));
        }

        public IReadOnlyCollection<ResolveFailure> Failures
            => _failures ?? NoFailures;

        internal ResolveCache Cache { get; set; }

        internal IHarshProvisionerContext ProvisionerContext { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveContext));

        private static readonly IReadOnlyCollection<ResolveFailure> NoFailures
            = new ResolveFailure[0];
    }
}
