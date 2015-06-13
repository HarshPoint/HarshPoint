using HarshPoint.Provisioning.Implementation;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingle<T>
    {
        Task<T> ResolveSingleAsync(IResolveContext context);
    }
}
