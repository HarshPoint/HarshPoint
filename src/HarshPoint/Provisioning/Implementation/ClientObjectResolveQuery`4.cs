using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal class ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier>
        : IClientObjectResolveQuery<TIntermediate, TIdentifier>
        where TIntermediate : ClientObject
    {
        public ClientObjectResolveQuery(
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<TIntermediate>> queryBuilder,
            Func<IEnumerable<TIntermediate>, IEnumerable<T>> postQueryTransform
        )
            : this(
                (_, obj) => identifierSelector(obj),
                queryBuilder,
                postQueryTransform
            )
        {
        }

        public ClientObjectResolveQuery(
            Func<TParent, T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<TIntermediate>> queryBuilder,
            Func<IEnumerable<TIntermediate>, IEnumerable<T>> postQueryTransform
        )
        {
            if (identifierSelector == null)
            {
                throw Error.ArgumentNull(nameof(identifierSelector));
            }

            if (queryBuilder == null)
            {
                throw Error.ArgumentNull(nameof(queryBuilder));
            }

            if (postQueryTransform == null)
            {
                throw Error.ArgumentNull(nameof(postQueryTransform));
            }

            ParentIncludes = ImmutableList<Expression<Func<TParent, Object>>>.Empty;

            IdentifierSelector = identifierSelector;
            PostQueryTransform = postQueryTransform;
            QueryBuilder = queryBuilder;
        }

        public IQueryable<TIntermediate> CreateQuery(TParent parent, ResolveContext<HarshProvisionerContext> resolveContext)
        {
            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            var query = QueryBuilder(parent);

#if true // [Obsolete]
            var clientObjectResolveContext = (resolveContext as ClientObjectResolveContext);
            if (clientObjectResolveContext != null)
            {
                var transformer = clientObjectResolveContext.QueryProcessor;
                var withAddedRetrievals = transformer.Process(query.Expression);

                query = query.Provider.CreateQuery<TIntermediate>(withAddedRetrievals);
            }
#endif

            return query;
        }

        public ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier> WithIdentifierComparer(IEqualityComparer<TIdentifier> identifierComparer)
        {
            IdentifierComparer = identifierComparer ?? EqualityComparer<TIdentifier>.Default;
            return this;
        }

        public ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier> WithIncludesOnParent(params Expression<Func<TParent, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Error.ArgumentNull(nameof(retrievals));
            }

            ParentIncludes = ParentIncludes.AddRange(retrievals);
            return this;
        }

        public IEqualityComparer<TIdentifier> IdentifierComparer
        {
            get;
            private set;
        }

        public Func<TParent, T, TIdentifier> IdentifierSelector
        {
            get;
            private set;
        }

        public IImmutableList<Expression<Func<TParent, Object>>> ParentIncludes
        {
            get;
            private set;
        }

        public Func<IEnumerable<TIntermediate>, IEnumerable<T>> PostQueryTransform
        {
            get;
            private set;
        }

        private Func<TParent, IQueryable<TIntermediate>> QueryBuilder
        {
            get;
            set;
        }
    }

    internal class ClientObjectResolveQuery<T, TParent, TIdentifier> :
        ClientObjectResolveQuery<T, T, TParent, TIdentifier>
        where T : ClientObject
    {
        public ClientObjectResolveQuery(
            Func<TParent, T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<T>> queryBuilder
        )
            : base(
                identifierSelector,
                queryBuilder,
                results => results
            )
        {
        }

        public ClientObjectResolveQuery(
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<T>> queryBuilder
        )
            : base(
                  identifierSelector,
                  queryBuilder,
                  results => results
            )
        {
        }
    }
}
