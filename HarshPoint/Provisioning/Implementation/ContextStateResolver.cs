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
                Resolve(context)
            );
        }

        public Task<T> ResolveSingleOrDefaultAsync(HarshProvisionerContextBase context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return Task.FromResult(
                Resolve(context).FirstOrDefault()
            );
        }

        private static IEnumerable<T> Resolve(HarshProvisionerContextBase context)
        {
            var results = new List<T>();

            foreach (var state in context.StateStack)
            {
                var typedCollection = (state as IEnumerable<T>);

                if (typedCollection != null)
                {
                    results.AddRange(typedCollection);
                }
                else if (state is T)
                {
                    results.Add((T)(state));
                }
            }

            return results;
        }
    }
}
