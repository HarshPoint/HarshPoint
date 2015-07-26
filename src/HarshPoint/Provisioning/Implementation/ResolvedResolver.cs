using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolvedResolver<T> : IResolveOld<T>
    {
        public ResolvedResolver(IEnumerable<T> values)
        {
            if (values == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(values));
            }

            Values = values.ToImmutableList();
        }

        public Task<IEnumerable<T>> TryResolveAsync(IResolveContext context)
        {
            return Task.FromResult(Values);
        }

        private IEnumerable<T> Values
        {
            get;
            set;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolvedResolver<>));
    }
}
