using System;
using System.Collections;
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
            return ThisAsResolveBuilderOfTContext.Initialize(
                ValidateContext(context)
            );
        }

        void IResolveBuilder.InitializeContext(IResolveContext context)
        {
            ThisAsResolveBuilderOfTContext.InitializeContext(
                ValidateContext(context)
            );
        }

        IEnumerable IResolveBuilder.ToEnumerable(Object state, IResolveContext context)
        {
            return ThisAsResolveBuilderOfTContext.ToEnumerable(
                state,
                ValidateContext(context)
            );
        }

        Object IResolveBuilder<TContext>.Initialize(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Elements.Select(e => e.ElementInitialize(context)).ToArray();
        }

        void IResolveBuilder<TContext>.InitializeContext(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            foreach (var element in Elements)
            {
                element.ElementInitializeContext(context);
            }
        }

        IEnumerable IResolveBuilder<TContext>.ToEnumerable(Object state, TContext context)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var elementStates = (state as Object[]);

            if (elementStates == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(nameof(state), state, typeof(Object[]));
            }

            var elementCount = Elements.Count();

            if (elementStates.Length != elementCount)
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(state),
                    SR.ClientObjectResolveBuilder_StateCountNotEqualToElementCount,
                    elementStates.Length,
                    elementCount
                );
            }

            return Elements.SelectMany(
                (e, i) => e.ElementToEnumerable(elementStates[i], context).Cast<Object>()
            );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw CannotCallThisMethod();
        }

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
        {
            throw CannotCallThisMethod();
        }

        TResult IResolveSingle<TResult>.Value
        {
            get { throw CannotCallThisMethod(); }
        }

        TResult IResolveSingleOrDefault<TResult>.Value
        {
            get { throw CannotCallThisMethod(); }
        }

        public ResolveBuilder<TResult, TContext> And(Chain<IResolveBuilderElement<TContext>> other)
        {
            Append(other);
            return this;
        }

        private IResolveBuilder<TContext> ThisAsResolveBuilderOfTContext
            => (this);

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

        private static Exception CannotCallThisMethod()
        {
            return Logger.Fatal.InvalidOperation(
                SR.ResolveBuilder_CannotCallThisMethod
            );
        }
    }
}