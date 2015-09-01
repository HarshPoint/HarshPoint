using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
        void InitializeContext(ResolveContext context);

        Object Initialize(ResolveContext context);

        IEnumerable<Object> ToEnumerable(ResolveContext context, Object state);
    }
}
