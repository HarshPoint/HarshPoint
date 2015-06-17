using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ContextStateResolver<T> : IResolve<T>
    {
        public Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }
            
            return Task.FromResult(
                context.ProvisionerContext.GetState<T>()
            );
        }
    }
}
