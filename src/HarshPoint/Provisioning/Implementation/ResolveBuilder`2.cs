using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract partial class ResolveBuilder<TResult, TContext> :
        Chain<IResolveBuilderElement<TContext>>,
        IResolveBuilderElement<TContext>,
        IResolveBuilder<TResult, TContext>
        where TContext : ResolveContext
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilder<,>));

        Object IResolveBuilder.Initialize(ResolveContext context)
        {
            var typedContext = ValidateContext(context);

            var result = TryGetFromCache(context);

            if (result != null)
            {
                return result;
            }

            return Elements
                .Select(e => e.ElementInitialize(typedContext))
                .ToArray();
        }

        void IResolveBuilder.InitializeContext(ResolveContext context)
        {
            var typedContext = ValidateContext(context);

            if (TryGetFromCache(context) != null)
            {
                // do not initialize context when loading from cache
                return;
            }

            foreach (var element in Elements)
            {
                element.ElementInitializeContext(typedContext);
            }
        }

        IEnumerable<Object> IResolveBuilder.ToEnumerable(
            ResolveContext context, 
            Object state
        )
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var typedContext = ValidateContext(context);
            var elementStates = (state as Object[]);

            if (elementStates == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(nameof(state), state, typeof(Object[]));
            }

            var elementCount = Elements.Count();

            if (elementStates.Length != elementCount)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(state),
                    SR.ClientObjectResolveBuilder_StateCountNotEqualToElementCount,
                    elementStates.Length,
                    elementCount
                );
            }

            return Elements.SelectMany(
                (e, i) => e.ElementToEnumerable(typedContext, elementStates[i]).Cast<Object>()
            );
        }

        public ResolveBuilder<TResult, TContext> And(Chain<IResolveBuilderElement<TContext>> other)
            => (ResolveBuilder<TResult, TContext>)Append(other);

        private Object TryGetFromCache(ResolveContext context) 
            => context.Cache?.TryGetValue(this);

        private static TContext ValidateContext(ResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var typed = (context as TContext);

            if (typed == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(context),
                    context,
                    typeof(TContext)
                );
            }

            return typed;
        }
    }
}