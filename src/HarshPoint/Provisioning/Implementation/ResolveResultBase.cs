using System.Collections;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultBase
    {
        internal IResolveBuilder ResolveBuilder
        {
            get;
            set;
        }

        internal IEnumerable Results
        {
            get;
            set;
        }

        protected IImmutableList<T> EnumerateResults<T>()
            => (Results ?? new T[0]).Cast<T>().ToImmutableArray();
    }
}
