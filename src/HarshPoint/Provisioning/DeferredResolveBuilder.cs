using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public static class DeferredResolveBuilder
    {
        public static IResolveBuilder<TResult, TContext> Create<TResult, TContext>(Func<IResolveBuilder<TResult, TContext>> factory)
            where TContext : class, IResolveContext
        {
            return new DeferredResolveBuilder<TResult, TContext>(factory);
        }
    }
}
