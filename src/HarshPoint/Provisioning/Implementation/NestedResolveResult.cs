using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public class NestedResolveResult
    {
        protected NestedResolveResult(Object value, IImmutableList<Object> parents)
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
            => ExtractComponents((IEnumerable<Type>)(componentTypes));

        public ImmutableArray<Object> ExtractComponents(IEnumerable<TypeInfo> componentTypes)
        {
            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            return ExtractComponents(componentTypes.ToArray());
        }

        public ImmutableArray<Object> ExtractComponents(IEnumerable<Type> componentTypes)
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

            if (!componentTypes.Last().IsAssignableFrom(ValueType.GetTypeInfo()))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(componentTypes),
                    SR.NestedResoveResult_LastComponentTypeMustBeAssignableFromT,
                    componentTypes.Last(),
                    ValueType
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

        public Tuple<T1, T2> ExtractComponents<T1, T2>()
        {
            var components = ExtractComponents(typeof(T1), typeof(T2));

            return Tuple.Create(
                (T1)components[0],
                (T2)components[1]
            );
        }

        public Object Value { get; private set; }

        public IImmutableList<Object> Parents { get; private set; }

        public virtual Type ValueType => Value.GetType();

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

            var parentNested = (parent as NestedResolveResult);

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

        public static Object Unpack(Object value)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            var nested = (value as NestedResolveResult);

            if (nested != null)
            {
                return nested.Value;
            }

            return value;
        }

        public static T Unpack<T>(Object value)
            => (T)Unpack(value);

        public static NestedResolveResult FromComponents(IEnumerable<Object> components)
        {
            if (components == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(components));
            }

            if (!components.Any())
            {
                throw Logger.Fatal.ArgumentEmptySequence(nameof(components));
            }

            var array = components.ToImmutableArray();
            var value = array.Last();

            if (value == null)
            {
                throw Logger.Fatal.Argument(
                    nameof(components),
                    SR.NestedResoveResult_LastComponentCannotBeNull
                );
            }

            return new NestedResolveResult(
                value,
                array.Take(array.Length - 1).ToImmutableArray()
            );
        }

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

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveResult));
    }
}
