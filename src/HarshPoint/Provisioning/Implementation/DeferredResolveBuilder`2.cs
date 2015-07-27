using System;
using System.Collections;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    /// <remarks>Does not inherit from <see cref="ResolveBuilder{TResult, TContext}" />
    /// because that one inherits from <see cref="Chain{TElement}"/> and we don't want
    /// that functionality here.</remarks>
    internal sealed partial class DeferredResolveBuilder<TResult, TContext> : 
        IResolveBuilder<TResult, TContext>
        where TContext : class, IResolveContext
    {
        private readonly Func<IResolveBuilder<TResult, TContext>> _factory;

        private IResolveBuilder<TResult, TContext> _inner;

        public DeferredResolveBuilder(Func<IResolveBuilder<TResult, TContext>> factory)
        {
            if (factory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(factory));
            }

            _factory = factory;
        }
       
        Object IResolveBuilder.Initialize(IResolveContext context)
        {
            return Inner.Initialize(context);
        }

        Object IResolveBuilder<TContext>.Initialize(TContext context)
        {
            return Inner.Initialize(context);
        }

        void IResolveBuilder.InitializeContext(IResolveContext context)
        {
            InitializeInner(context);
        }

        void IResolveBuilder<TContext>.InitializeContext(TContext context)
        {
            InitializeInner(context);
        }

        IEnumerable IResolveBuilder.ToEnumerable(Object state, IResolveContext context)
        {
            return Inner.ToEnumerable(state, context);
        }

        IEnumerable<Object> IResolveBuilder<TContext>.ToEnumerable(Object state, TContext context)
        {
            return Inner.ToEnumerable(state, context);
        }

        private void InitializeInner(IResolveContext context)
        {
            _inner = _factory();
            _inner.InitializeContext(context);
        }

        private IResolveBuilder<TResult, TContext> Inner
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

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(DeferredResolveBuilder<,>));
    }
}
