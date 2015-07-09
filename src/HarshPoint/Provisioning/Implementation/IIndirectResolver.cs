using System;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IIndirectResolver
    {
        Object Initialize(IResolveContext context);
    }
}
