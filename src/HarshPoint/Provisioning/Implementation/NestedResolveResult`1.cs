using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class NestedResolveResult<T> : INestedResolveResult
    {
        public NestedResolveResult(T value, IImmutableList<Object> parents)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            if (parents == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parents));
            }

            Value = value;
            Parents = parents;
        }

        public ImmutableArray<Object> ExtractComponents(params Type[] componentTypes)
        {
            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            return ExtractComponents(
                componentTypes
                .Select(IntrospectionExtensions.GetTypeInfo)
                .ToArray()
            );
        }

        public ImmutableArray<Object> ExtractComponents(params TypeInfo[] componentTypes)
        {
            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            if (!componentTypes.Any())
            {
                throw Logger.Fatal.ArgumentEmptySequence(nameof(componentTypes));
            }

            if (!componentTypes.Last().IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(componentTypes),
                    SR.NestedResoveResult_LastComponentTypeMustBeAssignableFromT,
                    componentTypes.Last(),
                    typeof(T)
                );
            }

            var array = new Object[componentTypes.Length];

            using (var enumerator = Parents.GetEnumerator())
            {
                var atEnd = false;
                
                for (var i = 0; !atEnd && (i < array.Length - 1); i++)
                {
                    array[i] = NextOfType(enumerator, componentTypes[i], out atEnd);
                }

                array[array.Length - 1] = Value;
            }

            return array.ToImmutableArray();
        }

        public T Value { get; private set; }

        public IImmutableList<Object> Parents { get; private set; }

        Object INestedResolveResult.Value => Value;

        private static Object NextOfType(IEnumerator enumerator, TypeInfo typeInfo, out Boolean atEnd)
        {
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                if (current == null)
                {
                    continue;
                }

                var currentTypeInfo = current.GetType().GetTypeInfo();

                if (typeInfo.IsAssignableFrom(currentTypeInfo))
                {
                    atEnd = false;
                    return current;
                }
            }

            atEnd = true;
            return null;
        }

        private static HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveResult<>));
    }
}
