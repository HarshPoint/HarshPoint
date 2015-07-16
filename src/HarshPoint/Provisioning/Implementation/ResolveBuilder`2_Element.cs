using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ResolveBuilder<TResult, TContext>
    {
        void IResolveBuilderElement<TResult, TContext>.InitializeContext(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            InitializeContext(context);
        }

        Object IResolveBuilderElement<TResult, TContext>.Initialize(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Initialize(context);
        }

        IEnumerable<TResult> IResolveBuilderElement<TResult, TContext>.ToEnumerable(Object state, TContext context)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ToEnumerable(state, context);
        }

        protected virtual void InitializeContext(TContext context)
        {
        }

        protected abstract Object Initialize(TContext context);

        protected abstract IEnumerable<TResult> ToEnumerable(Object state, TContext context);
    }
}
