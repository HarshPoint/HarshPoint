using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning
{
    public class ResolveContext<TProvisionerContext> : IResolveContext
        where TProvisionerContext : IHarshProvisionerContext
    {
        private List<ResolveFailure> _failures;

        public ResolveContext(TProvisionerContext provisionerContext)
        {
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

        public TProvisionerContext ProvisionerContext
        {
            get;

        }

        IHarshProvisionerContext IResolveContext.ProvisionerContext => ProvisionerContext;

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveContext<>));

        private static readonly IReadOnlyCollection<ResolveFailure> NoFailures = new ResolveFailure[0];
    }
}
