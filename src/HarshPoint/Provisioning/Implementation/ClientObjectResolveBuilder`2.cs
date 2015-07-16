using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult, TQueryResult> :
        ResolveBuilder<TResult, ClientObjectResolveContext>
        where TResult : ClientObject
        where TQueryResult : ClientObject
    {
        protected override Object Initialize(ClientObjectResolveContext context)
        {
            var query = CreateQuery(context);

            if (query == null)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.ClientObjectResolveBuilder_ToQueryableReturnedNull,
                    GetType()
                );
            }

            return context.LoadQuery(
                query
            );
        }

        protected override IEnumerable<TResult> ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var enumerable = (state as IEnumerable<TQueryResult>);

            if (enumerable == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(state),
                    state,
                    typeof(IEnumerable<TQueryResult>)
                );
            }

            return TransformQueryResults(enumerable, context);
        }

        protected virtual IQueryable<TQueryResult> CreateQuery(
            ClientObjectResolveContext context
        )
        {
            throw Logger.Fatal.NotImplemented();
        }

        protected virtual IEnumerable<TResult> TransformQueryResults(
            IEnumerable<TQueryResult> results,
            ClientObjectResolveContext context
        )
        {
            throw Logger.Fatal.NotImplemented();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientObjectResolveBuilder<,>));
    }
}