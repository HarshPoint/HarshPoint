using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract partial class ResolveBuilder<TResult, TContext> :
        Chain<IResolveBuilderElement<TContext>>,
        IResolveBuilderElement<TContext>,
        IResolveBuilder<TResult, TContext>
        where TContext : class, IResolveContext
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilder<,>));

        Object IResolveBuilder.Initialize(IResolveContext context)
        {
            var typedContext = ValidateContext(context);

            return Elements
                .Select(e => e.ElementInitialize(typedContext))
                .ToArray();
        }

        void IResolveBuilder.InitializeContext(IResolveContext context)
        {
            var typedContext = ValidateContext(context);

            foreach (var element in Elements)
            {
                element.ElementInitializeContext(typedContext);
            }
        }

        IEnumerable<Object> IResolveBuilder.ToEnumerable(IResolveContext context, Object state)
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
        {
            Append(other);
            return this;
        }

        private static TContext ValidateContext(IResolveContext context)
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