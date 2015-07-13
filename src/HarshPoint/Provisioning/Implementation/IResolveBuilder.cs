using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
    }

    public interface IResolveBuilder<TContext> : IResolveBuilder
        where TContext : HarshProvisionerContextBase
    {
        Object Initialize(ResolveContext<TContext> context);

        IEnumerable ToEnumerable(Object state, ResolveContext<TContext> context);
    }
}
