using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning
{
    public abstract class ResolveContext
    {
        private readonly List<ResolveFailure> _failures = new List<ResolveFailure>();

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

            _failures.Add(new ResolveFailure(resolveBuilder, identifier));
        }

        public IReadOnlyCollection<ResolveFailure> Failures
            => _failures;

        internal ResolveCache Cache { get; set; }

        internal IHarshProvisionerContext ProvisionerContext { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveContext));
    }
}
