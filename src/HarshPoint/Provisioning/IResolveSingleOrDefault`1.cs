using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingleOrDefault<out T> : IResolveBuilder
    {
        T Value { get; }
    }
}
