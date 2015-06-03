using HarshPoint.Provisioning.Implementation;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public interface IResolveSingle<T>
    {
        Task<T> ResolveSingleOrDefaultAsync(HarshProvisionerContextBase context);
    }

    public static class ResolveSingleExtensions
    {
        public static async Task<T> ResolveSingleAsync<T>(this IResolveSingle<T> resolver, HarshProvisionerContextBase context)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            var result = await resolver.ResolveSingleOrDefaultAsync(context);

            if (result == null)
            {
                throw Error.InvalidOperation(SR.Resolvable_NoResult);
            }

            return result;
        }
    }
}
