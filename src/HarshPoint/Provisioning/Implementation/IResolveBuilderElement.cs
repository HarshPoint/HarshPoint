using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilderElement<out TResult, TContext>
        where TContext : IResolveContext
    {
        void InitializeContext(TContext context);

        Object Initialize(TContext context);

        IEnumerable<TResult> ToEnumerable(Object state, TContext context);
    }
}