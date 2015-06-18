using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface IResolvableIdentifiers<TIdentifier>
    {
        IImmutableList<TIdentifier> Identifiers
        {
            get;
        }
    }

    internal static class IResolvableIdentifiersExtensions
    {
        public static IEnumerable<T> ResolveItems<T, TIdentifier>(
            this IResolvableIdentifiers<TIdentifier> resolvable,
            IResolveContext context,
            IEnumerable<T> items,
            Func<T, TIdentifier> idSelector,
            IEqualityComparer<TIdentifier> idComparer = null
        )
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (items == null)
            {
                throw Error.ArgumentNull(nameof(items));
            }

            if (idSelector == null)
            {
                throw Error.ArgumentNull(nameof(idSelector));
            }

            var byId = items.ToImmutableDictionary(idSelector, idComparer);

            foreach (var id in resolvable.Identifiers)
            {
                T value;

                if (byId.TryGetValue(id, out value))
                {
                    yield return value;
                }
                else
                {
                    context.AddFailure(resolvable, id);
                }
            }
        }

        public static async Task<IEnumerable<T>> ResolveQuery<T, TIntermediate, TParent, TIdentifier>(
            this IResolvableIdentifiers<TIdentifier> resolvable,
            ClientObjectResolveQuery<T, TIntermediate, TParent, TIdentifier> resolveQuery,
            ResolveContext<HarshProvisionerContext> context,
            TParent parent
        )
            where T : ClientObject
            where TIntermediate : ClientObject
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (resolveQuery == null)
            {
                throw Error.ArgumentNull(nameof(resolveQuery));
            }

            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            var query = resolveQuery.CreateQuery(parent, context);
            var clientContext = context.ProvisionerContext.ClientContext;

            var intermediate = clientContext.LoadQuery(query);
            await clientContext.ExecuteQueryAsync();

            var items = resolveQuery.PostQueryTransform(intermediate);

            return resolvable.ResolveItems(
                context,
                items,
                resolveQuery.IdentifierSelector,
                resolveQuery.IdentifierComparer
            );
        }
    }
}
