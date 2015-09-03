using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultBase
    {
        private ImmutableArray<Object> _cached;

        public IResolveBuilder ResolveBuilder
        {
            get;
            internal set;
        }

        public ResolveContext ResolveContext
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
            => EnumerateResults<T>(allowFailures: false);

        protected IImmutableList<T> EnumerateResults<T>(Boolean  allowFailures)
        {
            if (_cached.IsDefault)
            {
                _cached = (Results ?? new Object[0])
                    .Cast<Object>()
                    .ToImmutableArray();
            }

            if (!allowFailures)
            {
                ValidateNoFailures();
            }

            return _cached.Cast<T>().ToImmutableArray();
        }


        protected void ValidateNoFailures()
        {
            if (ResolveContext.Failures.Any())
            {
                throw Logger.Fatal.Write(
                    new ResolveFailedException(ResolveContext.Failures)
                );
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultBase>();
    }
}
