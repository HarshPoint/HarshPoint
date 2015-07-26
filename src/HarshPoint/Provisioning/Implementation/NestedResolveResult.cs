using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class NestedResolveResult
    {
        public static NestedResolveResult<T> Pack<T>(T value, Object parent)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            var parentNested = (parent as INestedResolveResult);

            if (parentNested != null)
            {
                return new NestedResolveResult<T>(
                    value,
                    parentNested.Parents.Add(parentNested.Value)
                );
            }

            return new NestedResolveResult<T>(
                value,
                ImmutableList.Create(parent)
            );
        }

        public static Object Unpack(Object obj)
        {
            if (obj == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(obj));
            }

            var nested = (obj as INestedResolveResult);

            if (nested != null)
            {
                return nested.Value;
            }

            return obj;
        }

        public static T Unpack<T>(Object obj)
            => (T)Unpack(obj);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveResult));
    }
}
