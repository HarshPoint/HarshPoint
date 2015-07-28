using System.Collections.Generic;

namespace HarshPoint.Provisioning
{
    public interface IResolve<out T> : IEnumerable<T>
    {
    }
}
