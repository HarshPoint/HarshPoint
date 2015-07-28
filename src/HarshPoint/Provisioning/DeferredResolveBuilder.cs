using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public static class DeferredResolveBuilder
    {
        public static IResolveBuilder<TResult> Create<TResult>(Func<IResolveBuilder<TResult>> factory)
        {
            return new DeferredResolveBuilder<TResult>(factory);
        }
    }
}
