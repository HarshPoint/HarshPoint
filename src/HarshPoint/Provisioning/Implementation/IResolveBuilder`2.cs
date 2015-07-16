using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<out TResult, TContext> : IResolveBuilder<TContext>
        where TContext : IResolveContext
    {
        new IEnumerable<TResult> ToEnumerable(Object state, TContext context);
    }
}
