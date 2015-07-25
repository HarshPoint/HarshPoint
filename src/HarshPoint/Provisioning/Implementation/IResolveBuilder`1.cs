using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<TContext> : IResolveBuilder
        where TContext : IResolveContext
    {
        void InitializeContext(TContext context);

        Object Initialize(TContext context);

        IEnumerable<Object> ToEnumerable(Object state, TContext context);
    }
}
