using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    internal sealed class ClientObjectResolveQuery<T, TParent, TIdentifier>
        where T : ClientObject
    {
        public ClientObjectResolveQuery(
            Func<T, TIdentifier> identifierSelector,
            Func<TParent, IQueryable<T>> queryBuilder,
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

            IdentifierComparer = identifierComparer ?? EqualityComparer<TIdentifier>.Default;
            IdentifierSelector = identifierSelector;
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

        public Func<TParent, IQueryable<T>> QueryBuilder
        {
            get;
            private set;
        }
    }
}
