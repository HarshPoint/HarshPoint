using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IQueryable<TIntermediate> CreateQuery(TParent parent, ResolveContext<HarshProvisionerContext> resolveContext)
        {
            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            var query = QueryBuilder(parent);
            var clientObjectResolveContext = (resolveContext as ClientObjectResolveContext);

            if (clientObjectResolveContext != null)
            {
                var transformer = new ClientObjectResolveQueryProcessor(clientObjectResolveContext);
                var withAddedRetrievals = transformer.AddContextRetrievals(query.Expression);

                query = query.Provider.CreateQuery<TIntermediate>(withAddedRetrievals);
            }

            return query;
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
