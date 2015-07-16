using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder
    {
        void InitializeContext(IResolveContext context);

        Object Initialize(IResolveContext context);

        IEnumerable ToEnumerable(Object state, IResolveContext context);

    }
}
