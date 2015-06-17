using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal class ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier>
        where TIntermediate : ClientObject
    {
        public ClientObjectResolveQuery(
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<TIntermediate>> queryBuilder,
            Func<IEnumerable<TIntermediate>, IEnumerable<T>> postQueryTransform,
            IEqualityComparer<TIdentifier> identifierComparer = null
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

            IdentifierComparer = identifierComparer ?? EqualityComparer<TIdentifier>.Default;
            IdentifierSelector = identifierSelector;
            PostQueryTransform = postQueryTransform;
            QueryBuilder = queryBuilder;
        }

        public IQueryable<TIntermediate> CreateQuery(TParent parent, Expression<Func<T, Object>>[] retrievals)
        {
            var query = QueryBuilder(parent);

            var transformer = new ClientObjectResolveRetrievalTransformer<T>(retrievals);
            var replacedRetrievals = transformer.Process(query.Expression);

            return query.Provider.CreateQuery<TIntermediate>(replacedRetrievals);
        }

        public IEqualityComparer<TIdentifier> IdentifierComparer
        {
            get;
            private set;
        }

        public Func<T, TIdentifier> IdentifierSelector
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
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<T>> queryBuilder,
            IEqualityComparer<TIdentifier> identifierComparer = null
        )
            : base(
                  identifierSelector,
                  queryBuilder,
                  results => results,
                  identifierComparer
            )
        {
        }
    }
}
