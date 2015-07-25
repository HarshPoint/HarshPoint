using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilderElement<TContext>
        where TContext : IResolveContext
    {
        void ElementInitializeContext(TContext context);

        Object ElementInitialize(TContext context);

        IEnumerable ElementToEnumerable(Object state, TContext context);
    }
}