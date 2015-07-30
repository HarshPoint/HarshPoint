using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ClientObjectResolveBuilder<TResult, TQueryResult> :
        ResolveBuilder<TResult, ClientObjectResolveContext>
        where TResult : ClientObject
        where TQueryResult : ClientObject
    {
        protected sealed override Object Initialize(ClientObjectResolveContext context)
        {
            var queries = CreateQueries(context);

            if (queries == null)
            {
                return new IEnumerable<TQueryResult>[0];
            }

            return queries
                .Select(context.LoadQuery)
                .ToArray();
        }

        protected sealed override IEnumerable ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var enumerables = (state as IEnumerable<TQueryResult>[]);

            if (enumerables == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(state),
                    state,
                    typeof(IEnumerable<TQueryResult>[])
                );
            }

            return enumerables.SelectMany(
                enumerable => TransformQueryResults(enumerable, context)
            );
        }

        protected virtual IQueryable<TQueryResult> CreateQuery(
            ClientObjectResolveContext context
        )
        {
            throw Logger.Fatal.NotImplemented();
        }

        protected virtual IQueryable<TQueryResult>[] CreateQueries(
            ClientObjectResolveContext context
        )
            => new[] { CreateQuery(context) };

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