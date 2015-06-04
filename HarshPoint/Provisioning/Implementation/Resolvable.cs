using System;
using System.Collections.Generic;
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
    }
}
