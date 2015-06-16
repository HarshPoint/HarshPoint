using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal class ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier>
        where TIntermediate : ClientObject
    {
        public ClientObjectResolveQuery(
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, Expression<Func<T, Object>>[], IQueryable<TIntermediate>> queryBuilder,
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

        public Func<TParent, Expression<Func<T, Object>>[], IQueryable<TIntermediate>> QueryBuilder
        {
            get;
            private set;
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
                  (parent, retrievals) => queryBuilder(parent).Include(retrievals),
                  results => results,
                  identifierComparer
            )
        {
        }
    }
}
