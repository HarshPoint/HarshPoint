using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning
{
    public interface IResolveContext
    {
        void AddFailure(IResolveBuilder resolveBuilder, Object identifier);

        IReadOnlyCollection<ResolveFailure> Failures { get; }

        IHarshProvisionerContext ProvisionerContext { get; }
    }
}
