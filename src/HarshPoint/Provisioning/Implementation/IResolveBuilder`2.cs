using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<out TResult, TContext> : 
        IResolveBuilder<TContext>,
        IResolve<TResult>,
        IResolveSingle<TResult>,
        IResolveSingleOrDefault<TResult>
        where TContext : IResolveContext
    {
        new IEnumerable<TResult> ToEnumerable(Object state, TContext context);
    }
}
