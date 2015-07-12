using System;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete]
    public interface IIndirectResolver
    {
        Object Initialize(IResolveContext context);
    }
}
