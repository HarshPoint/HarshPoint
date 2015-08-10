using System;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingleOrDefault<out T>
    {
        Boolean HasValue { get; }
        T Value { get; }
    }
}
