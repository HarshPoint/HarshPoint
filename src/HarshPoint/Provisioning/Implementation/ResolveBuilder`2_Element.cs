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

            return Initialize(context);
        }

        IEnumerable IResolveBuilderElement<TContext>.ElementToEnumerable(TContext context, Object state)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ToEnumerable(state, context);
        }

        protected virtual void InitializeContext(TContext context)
        {
        }

        protected virtual Object Initialize(TContext context) => null;

        protected abstract IEnumerable ToEnumerable(Object state, TContext context);
    }
}
