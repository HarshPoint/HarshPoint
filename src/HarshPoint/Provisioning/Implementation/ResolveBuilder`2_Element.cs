using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ResolveBuilder<TResult, TContext>
    {
        void IResolveBuilderElement<TContext>.ElementInitializeContext(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            InitializeContext(context);
        }

        Object IResolveBuilderElement<TContext>.ElementInitialize(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var cached = context.Cache?.TryGetValue(this);

            if (cached != null)
            {
                InitializeCached(context, cached);

                return new CachedResult()
                {
                    Values = cached
                };
            }

            return Initialize(context);
        }

        IEnumerable IResolveBuilderElement<TContext>.ElementToEnumerable(TContext context, Object state)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var cached = (state as CachedResult);

            if (cached != null)
            {
                return cached.Values;
            }

            var result = ToEnumerable(state, context);
            context.Cache?.SetValue(this, result);
            return result;
        }

        protected virtual void InitializeContext(TContext context)
        {
        }

        protected virtual Object Initialize(TContext context) => null;

        protected virtual void InitializeCached(
            TContext context,
            IEnumerable enumerable
        )
        {
        }

        protected abstract IEnumerable ToEnumerable(Object state, TContext context);

        private sealed class CachedResult
        {
            public IEnumerable Values { get; set; }
        }
    }
}
