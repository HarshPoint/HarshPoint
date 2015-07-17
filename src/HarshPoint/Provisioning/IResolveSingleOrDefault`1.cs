using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingleOrDefault<out T> : IResolveBuilder
    {
        T Value { get; }
    }
}
