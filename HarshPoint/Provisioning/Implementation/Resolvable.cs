using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        public static T EnsureSingleOrDefault<T>(Object resolvable, IEnumerable<T> values)
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (values == null)
            {
                throw Error.ArgumentNull(nameof(values));
            }

            switch (values.Count())
            {
                case 0: return default(T);
                case 1: return values.First();
                default: throw Error.InvalidOperation(SR.Resolvable_ManyResults, resolvable);
            }
        }

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

                if ((definition == typeof(IResolve<>)) ||
                    (definition == typeof(IResolveSingle<>)))
                {
                    return info.GenericTypeArguments[0];
                }
            }

            return null;
        }

        public static IEnumerable<T> ResolveItems<T, TIdentifier, TProvisionerContext>(
            Object resolvable,
            ResolveContext<TProvisionerContext> context,
            IEnumerable<TIdentifier> identifiers,
            IEnumerable<T> items,
            Func<T, TIdentifier> idSelector,
            IEqualityComparer<TIdentifier> idComparer = null
        )
            where TProvisionerContext : HarshProvisionerContextBase
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

            return identifiers.Select(id =>
            {
                T value;

                if (!byId.TryGetValue(id, out value))
                {
                    context.AddFailure(resolvable, id);
                }

                return value;
            });
        }
    }
}
