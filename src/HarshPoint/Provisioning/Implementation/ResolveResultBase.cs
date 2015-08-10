using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultBase
    {
        public IResolveBuilder ResolveBuilder
        {
            get;
            internal set;
        }

        public IEnumerable<ResolveFailure> ResolveFailures
        {
            get;
            internal set;
        }

        internal IEnumerable Results
        {
            get;
            set;
        }

        protected IImmutableList<T> EnumerateResults<T>()
            => (Results ?? new T[0]).Cast<T>().ToImmutableArray();


        protected void ValidateNoFailures()
        {
            if (ResolveFailures?.Any() ?? false)
            {
                throw Logger.Fatal.Write(
                    new ResolveFailedException(ResolveFailures)
                );
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultBase>();
    }
}
