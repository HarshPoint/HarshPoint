using System;
using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveBuilderAdapter<TResult> : IResolveBuilder<TResult>
    {
        private readonly IResolveBuilder _inner;

        public ResolveBuilderAdapter(IResolveBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            _inner = builder;
        }

        void IResolveBuilder.InitializeContext(ResolveContext context)
            => _inner.InitializeContext(context);

        Object IResolveBuilder.Initialize(ResolveContext context)
            => _inner.Initialize(context);

        IEnumerable<Object> IResolveBuilder.ToEnumerable(ResolveContext context, Object state)
            => _inner.ToEnumerable(context, state);

        TResult IResolveSingle<TResult>.Value { get { throw CannotCall(); } }
        TResult IResolveSingleOrDefault<TResult>.Value { get { throw CannotCall(); } }
        Boolean IResolveSingleOrDefault<TResult>.HasValue { get { throw CannotCall(); } }
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() { throw CannotCall(); }
        IEnumerator IEnumerable.GetEnumerator() { throw CannotCall(); }

        private static Exception CannotCall()
            => Logger.Fatal.InvalidOperation(SR.ResolveBuilder_CannotCallThisMethod);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilderAdapter<>));
    }
}
