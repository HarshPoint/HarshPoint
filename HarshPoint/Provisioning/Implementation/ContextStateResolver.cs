using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolver<T> : IResolve<T>, IResolveSingle<T>
    {
        public Task<IEnumerable<T>> ResolveAsync(HarshProvisionerContextBase context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                context.GetState<T>()
            );
        }

        public Task<T> ResolveSingleOrDefaultAsync(HarshProvisionerContextBase context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                context.GetState<T>().FirstOrDefault()
            );
        }
    }
}
