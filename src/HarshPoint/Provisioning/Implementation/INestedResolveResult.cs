using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface INestedResolveResult
    {
        ImmutableArray<Object> ExtractComponents(params TypeInfo[] componentTypes);
        Object Value { get; }
        IImmutableList<Object> Parents { get; }
    }

    internal static class NestedResolveResultExtension
    {
        public static ImmutableArray<Object> ExtractComponents(
            this INestedResolveResult result,
            params Type[] componentTypes
        )
            => ExtractComponents(result, (IEnumerable<Type>)(componentTypes));


        public static ImmutableArray<Object> ExtractComponents(
            this INestedResolveResult result,
            IEnumerable<TypeInfo> componentTypes
        )
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            return result.ExtractComponents(componentTypes.ToArray());
        }

        public static ImmutableArray<Object> ExtractComponents(
            this INestedResolveResult result,
            IEnumerable<Type> componentTypes
        )
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            return result.ExtractComponents(
                componentTypes
                    .Select(IntrospectionExtensions.GetTypeInfo)
                    .ToArray()
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveResultExtension));
    }
}
