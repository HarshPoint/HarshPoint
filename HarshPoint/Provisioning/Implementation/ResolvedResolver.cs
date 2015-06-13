using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolvedResolver<T> : IResolve<T>, IResolveSingle<T>
    {
        public ResolvedResolver(IEnumerable<T> values)
        {
            if (values == null)
            {
                throw Error.ArgumentNull(nameof(values));
            }

            Values = values.ToImmutableList();
        }

        public Task<IEnumerable<T>> ResolveAsync(IResolveContext context)
        {
            return Task.FromResult(Values);
        }

        public Task<T> ResolveSingleAsync(IResolveContext context)
        {
            return Task.FromResult( 
                Resolvable.EnsureSingleOrDefault(this, Values)
            );
        }

        private IEnumerable<T> Values
        {
            get;
            set;
        }
    }
}
