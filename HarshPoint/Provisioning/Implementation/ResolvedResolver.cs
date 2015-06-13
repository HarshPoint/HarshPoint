using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolvedResolver<T> : IResolve<T>
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

        private IEnumerable<T> Values
        {
            get;
            set;
        }
    }
}
