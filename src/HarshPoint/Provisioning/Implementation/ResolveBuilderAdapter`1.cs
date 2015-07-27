using System;
using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveBuilderAdapter<T> : IResolveBuilderAdapter, IResolve<T>, IResolveSingle<T>, IResolveSingleOrDefault<T>
    {
        internal ResolveBuilderAdapter(IResolveBuilder builder) 
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            ResolveBuilder = builder;
        }

        internal IResolveBuilder ResolveBuilder { get; private set; }
        IResolveBuilder IResolveBuilderAdapter.ResolveBuilder => ResolveBuilder;

        T IResolveSingle<T>.Value { get { throw CannotCall(); } }
        T IResolveSingleOrDefault<T>.Value { get { throw CannotCall(); } }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() { throw CannotCall(); }
        IEnumerator IEnumerable.GetEnumerator() { throw CannotCall(); }

        private static Exception CannotCall()
            => Logger.Fatal.InvalidOperation(SR.ResolveBuilder_CannotCallThisMethod);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilderAdapter<>));
    }
}
