using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint
{
    public static class HarshTuple
    {
        public static Object Create(Type tupleType, IEnumerable<Object> items)
        {
            if (items == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(items));
            }

            return Create(tupleType, items.ToArray());
        }

        public static Object Create(Type tupleType, params Object[] items)
        {
            ValidateIsConstructedTupleType(tupleType);

            if (items == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(items));
            }

            if (!items.Any())
            {
                throw Logger.Fatal.ArgumentEmptySequence(nameof(items));
            }

            if (items.Length != tupleType.GenericTypeArguments.Length)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(items),
                    SR.HarshTuple_CreateTupleItemsCountDoesNotMatchType,
                    items.Length,
                    tupleType
                );
            }

            return Activator.CreateInstance(tupleType, items);
        }

        public static IImmutableList<Type> GetComponentTypes(Type tupleType)
        {
            ValidateIsConstructedTupleType(tupleType);

            return tupleType.GenericTypeArguments.ToImmutableArray();
        }

        public static Boolean IsTupleType(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (type.IsConstructedGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return TupleDefinitions.Contains(type);
        }

        public static Type MakeTupleType(IEnumerable<Type> componentTypes)
        {
            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            return MakeTupleType(componentTypes.ToArray());
        }

        public static Type MakeTupleType(params Type[] componentTypes)
        {
            if (componentTypes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(componentTypes));
            }

            if (!componentTypes.Any())
            {
                throw Logger.Fatal.ArgumentEmptySequence(nameof(componentTypes));
            }

            if (componentTypes.Length > MaxTupleComponents)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(componentTypes),
                    SR.HarshTuple_TooManyComponentTypes,
                    MaxTupleComponents
                );
            }

            var definition = TupleDefinitions[componentTypes.Length - 1];

            return definition.MakeGenericType(
                componentTypes
            );
        }

        private static Boolean IsRestTuple(Type tupleType)
            => IsRestTuple(tupleType.GenericTypeArguments);

        private static Boolean IsRestTuple(IEnumerable<Type> componentTypes)
            => (componentTypes.Count() == MaxTupleComponents) && IsTupleType(componentTypes.Last());
            
        private static void ValidateIsConstructedTupleType(Type tupleType)
        {
            if (tupleType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(tupleType));
            }

            if (!tupleType.IsConstructedGenericType)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(tupleType),
                    SR.HarshTuple_TypeIsNotAConstructedGenericType,
                    tupleType
                );
            }

            if (!IsTupleType(tupleType))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(tupleType),
                    SR.HarshTuple_TypeIsNotATuple,
                    tupleType
                );
            }
        }

        private static readonly ImmutableArray<Type> TupleDefinitions = ImmutableArray.Create(
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        );

        private static readonly Int32 MaxTupleComponents = TupleDefinitions.Length;

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshTuple));
    }
}
