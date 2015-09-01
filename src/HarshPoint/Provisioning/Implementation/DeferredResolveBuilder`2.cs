using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    /// <remarks>Does not inherit from <see cref="ResolveBuilder{TResult, TContext}" />
    /// because that one inherits from <see cref="Chain{TElement}"/> and we don't want
    /// that functionality here.</remarks>
    internal sealed partial class DeferredResolveBuilder<TResult> :
        IResolveBuilder<TResult>
    {
        private readonly Func<IResolveBuilder<TResult>> _factory;

        private IResolveBuilder<TResult> _inner;

        public DeferredResolveBuilder(Func<IResolveBuilder<TResult>> factory)
        {
            if (factory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(factory));
            }

            _factory = factory;
        }

        Object IResolveBuilder.Initialize(ResolveContext context)
            => Inner.Initialize(context);

        void IResolveBuilder.InitializeContext(ResolveContext context)
        {
            InitializeInner(context);
        }

        IEnumerable<Object> IResolveBuilder.ToEnumerable(ResolveContext context, Object state)
            => Inner.ToEnumerable(context, state);

        private void InitializeInner(ResolveContext context)
        {
            _inner = _factory();
            _inner.InitializeContext(context);
        }

        private IResolveBuilder<TResult> Inner
        {
            get
            {
                if (_inner == null)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.DeferredResolveBuilder_InnerNotInitialized
                    );
                }

                return _inner;
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(DeferredResolveBuilder<>));
    }
}
