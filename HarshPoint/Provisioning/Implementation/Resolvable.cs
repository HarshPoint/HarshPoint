using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        public static Type GetResolvedType(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw Error.ArgumentNull(nameof(interfaceType));
            }

            var info = interfaceType.GetTypeInfo();

            if (info.IsGenericType)
            {
                var definition = info.GetGenericTypeDefinition();

                if (definition == typeof(IResolve<>))
                {
                    return info.GenericTypeArguments[0];
                }
            }

            return null;
        }

        public static IEnumerable<T> ResolveItems<T, TIdentifier>(
            Object resolvable,
            IResolveContext context,
            IEnumerable<TIdentifier> identifiers,
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

            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
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

            foreach (var id in identifiers)
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
    }
}
