using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            IEnumerable<Tuple<TIdentifier, T>> items,
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

            var byId = items.ToImmutableDictionaryFirstWins(
                tuple => tuple.Item1,
                tuple => tuple.Item2,
                idComparer
            );

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
            params TParent[] parents
        )
            where T : ClientObject
            where TParent : ClientObject
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

            if (parents == null)
            {
                throw Error.ArgumentNull(nameof(parents));
            }

            var intermediates = new Dictionary<TParent, IEnumerable<TIntermediate>>();
            var clientContext = context.ProvisionerContext.ClientContext;

            foreach (var parent in parents)
            {
                var query = resolveQuery.CreateQuery(parent, context);

                if (resolveQuery.ParentIncludes.Any())
                {
                    clientContext.Load(parent, resolveQuery.ParentIncludes.ToArray());
                }

                intermediates.Add(
                    parent,
                    clientContext.LoadQuery(query)
                );
            }

            await clientContext.ExecuteQueryAsync();

            var items = intermediates.Select(
                results => ResolvedGrouping.Create(
                    results.Key,
                    resolveQuery.PostQueryTransform(results.Value)
                )
            );

            return resolvable.ResolveItems(
                context,
                from grouping in items
                from item in grouping
                select Tuple.Create(
                    resolveQuery.IdentifierSelector(grouping.Key, item),
                    item
                ),
                resolveQuery.IdentifierComparer
            );
        }
    }
}
