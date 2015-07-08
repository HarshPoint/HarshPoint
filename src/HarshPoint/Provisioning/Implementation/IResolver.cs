using System;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolver
    {
        Object Resolve(IResolveContext context);
    }
}
