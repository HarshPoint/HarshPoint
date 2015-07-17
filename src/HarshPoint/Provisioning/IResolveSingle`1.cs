using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingle<out T> : IResolveBuilder
    {
        T Value { get; }
    }
}
