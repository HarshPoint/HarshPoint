using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
        void InitializeContext(IResolveContext context);

        Object Initialize(IResolveContext context);

        IEnumerable<Object> ToEnumerable(Object state, IResolveContext context);
    }
}
