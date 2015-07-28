using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingle<out T>
    {
        T Value { get; }
    }
}
