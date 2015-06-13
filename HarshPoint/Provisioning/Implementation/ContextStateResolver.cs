using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolver<T> : IResolve<T>, IResolveSingle<T>
    {
        public Task<IEnumerable<T>> ResolveAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                context.ProvisionerContext.GetState<T>()
            );
        }

        public Task<T> ResolveSingleAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                context.ProvisionerContext.GetState<T>().FirstOrDefault()
            );
        }
    }
}
