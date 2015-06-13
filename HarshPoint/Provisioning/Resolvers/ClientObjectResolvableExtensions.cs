using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    internal static class ClientObjectResolvableExtensions
    {
        public static Task<IEnumerable<T>> ResolveClientObjectQuery<T, TIdentifier, TParent, TSelf>(
            this Resolvable<T, TIdentifier, HarshProvisionerContext, TSelf> resolvable,
            ResolveContext<HarshProvisionerContext> context,
            TParent parent,
            ClientObjectResolveQuery<T, TParent, TIdentifier> query
        )
            where T : ClientObject
            where TSelf : Resolvable<T, TIdentifier, HarshProvisionerContext, TSelf>
        {
            if (query == null)
            {
                throw Error.ArgumentNull(nameof(query));
            }

            return ResolveQuery(
                resolvable,
                context,
                resolvable.Identifiers,
                query.QueryBuilder(parent),
                query.IdentifierSelector,
                query.IdentifierComparer
            );
        }

        public static Task<IEnumerable<T2>> ResolveClientObjectQuery<T1, T2, TIdentifier, TParent, TSelf>(
            this NestedResolvable<T1, T2, TIdentifier, HarshProvisionerContext, TSelf> resolvable,
            ResolveContext<HarshProvisionerContext> context,
            TParent parent,
            ClientObjectResolveQuery<T2, TParent, TIdentifier> query
        )
            where T2 : ClientObject
            where TSelf : NestedResolvable<T1, T2, TIdentifier, HarshProvisionerContext, TSelf>
        {
            if (query == null)
            {
                throw Error.ArgumentNull(nameof(query));
            }

            return ResolveQuery(
                resolvable,
                context,
                resolvable.Identifiers,
                query.QueryBuilder(parent),
                query.IdentifierSelector,
                query.IdentifierComparer
            );
        }

        public static IEnumerable<T2> ResolveIdentifiers<T1, T2, TIdentifier, TSelf>(
            this NestedResolvable<T1, T2, TIdentifier, HarshProvisionerContext, TSelf> resolvable,
            ResolveContext<HarshProvisionerContext> context,
            IEnumerable<T2> items,
            Func<T2, TIdentifier> idSelector,
            IEqualityComparer<TIdentifier> idComparer = null
        )
            where T2 : ClientObject
            where TSelf : NestedResolvable<T1, T2, TIdentifier, HarshProvisionerContext, TSelf>
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            return Resolvable.ResolveItems(
                resolvable,
                context,
                resolvable.Identifiers,
                items,
                idSelector,
                idComparer
            );
        }

        private static async Task<IEnumerable<T>> ResolveQuery<T, TIdentifier>(
            Object resolvable,
            ResolveContext<HarshProvisionerContext> context,
            IEnumerable<TIdentifier> identifiers,
            IQueryable<T> query,
            Func<T, TIdentifier> idSelector,
            IEqualityComparer<TIdentifier> idComparer
        )
            where T : ClientObject
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            if (query == null)
            {
                throw Error.ArgumentNull(nameof(query));
            }

            if (idSelector == null)
            {
                throw Error.ArgumentNull(nameof(idSelector));
            }

            var items = context.ProvisionerContext.ClientContext.LoadQuery(query);
            await context.ProvisionerContext.ClientContext.ExecuteQueryAsync();

            return Resolvable.ResolveItems(
                resolvable,
                context,
                identifiers,
                items,
                idSelector,
                idComparer
            );
        }

    }
}
