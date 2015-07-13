using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult, TQueryResult, TSelf> :
        Chain<IClientObjectResolveBuilderElement<TResult>>,
        IClientObjectResolveBuilderElement<TResult>,
        IClientObjectResolveBuilder<TResult>
        where TQueryResult : ClientObject
        where TSelf : ClientObjectResolveBuilder<TResult, TQueryResult, TSelf>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<,,,>));

        private ImmutableList<Expression<Func<TResult, Object>>> _retrievals =
            ImmutableList<Expression<Func<TResult, Object>>>.Empty;

        Object IResolveBuilder<HarshProvisionerContext>.Initialize(ResolveContext<HarshProvisionerContext> context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Elements.Select(e => e.LoadQuery(context)).ToArray();
        }

        IEnumerable IResolveBuilder<HarshProvisionerContext>.ToEnumerable(Object state, ResolveContext<HarshProvisionerContext> context)
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
                (e, i) => e.TransformQueryResults(elementStates[i], context)
            );
        }

        Object IClientObjectResolveBuilderElement<TResult>.LoadQuery(ResolveContext<HarshProvisionerContext> context)
        {
            var query = CreateQuery(context);

            if (query == null)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.ClientObjectResolveBuilder_ToQueryableReturnedNull,
                    GetType()
                );
            }

            if (_retrievals.Any())
            {
                var queryProcessor = new ClientObjectResolveQueryProcessor(
                    typeof(TResult),
                    _retrievals
                );

                query = queryProcessor.AddRetrievals(query);
            }

            return context.ProvisionerContext.ClientContext.LoadQuery(query);
        }

        public TSelf And(Chain<IClientObjectResolveBuilderElement<TResult>> other)
        {
            Append(other);
            return (TSelf)(this);
        }

        public void Include(params Expression<Func<TResult, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            _retrievals = _retrievals.AddRange(retrievals);
        }

        protected abstract IQueryable<TQueryResult> CreateQuery(ResolveContext<HarshProvisionerContext> context);

        protected abstract IEnumerable<TResult> TransformQueryResults(IEnumerable<TQueryResult> queryResults, ResolveContext<HarshProvisionerContext> context);

        IEnumerable<TResult> IClientObjectResolveBuilderElement<TResult>.TransformQueryResults(Object state, ResolveContext<HarshProvisionerContext> context)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var queryResults = (state as IEnumerable<TQueryResult>);

            if (queryResults == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(state), 
                    state, 
                    typeof(IEnumerable<TQueryResult>)
                );
            }

            return TransformQueryResults(queryResults, context);
        }
    }
}
