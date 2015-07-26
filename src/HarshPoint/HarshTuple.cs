using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint
{
    public static class HarshTuple
    {
        public static IImmutableList<Type> GetComponentTypes(Type tupleType)
        {
            if (tupleType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(tupleType));
            }

            if (!tupleType.IsConstructedGenericType)
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(tupleType),
                    SR.HarshTuple_TypeIsNotAConstructedGenericType,
                    tupleType
                );
            }

            if (!IsTupleType(tupleType))
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(tupleType),
                    SR.HarshTuple_TypeIsNotATuple,
                    tupleType
                );
            }

            var componentTypes = tupleType.GenericTypeArguments.AsEnumerable();

            if ((componentTypes.Count() == MaxTupleComponents) &&
                IsTupleType(componentTypes.Last()))
            {
                componentTypes =
                    componentTypes
                    .Take(MaxTupleComponents - 1)
                    .Concat(
                        GetComponentTypes(componentTypes.Last())
                    );
            }

            return componentTypes.ToImmutableArray();
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
                componentTypes =
                    componentTypes
                    .Take(MaxTupleComponents - 1)
                    .Concat(
                        MakeTupleType(
                            componentTypes.Skip(MaxTupleComponents - 1)
                        )
                    )
                    .ToArray();
            }

            var definition = TupleDefinitions[componentTypes.Length - 1];

            return definition.MakeGenericType(
                componentTypes
            );
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
