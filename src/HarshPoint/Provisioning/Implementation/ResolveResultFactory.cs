using HarshPoint.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ResolveResultFactory
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultFactory));

        public static Object CreateResult(
            TypeInfo propertyTypeInfo,
            IEnumerable enumerable,
            IResolveBuilder builder,
            IEnumerable<ResolveFailure> failures
        )
        {
            if (propertyTypeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyTypeInfo));
            }

            if (enumerable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(enumerable));
            }

            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            if (failures == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(failures));
            }

            var property = ResolvedPropertyTypeInfo.Parse(propertyTypeInfo);

            var enumerableTypes =
                GetEnumerableTypes(enumerable.GetType())
                .ToArray();

            if (!enumerableTypes.Contains(property.ResolvedType))
            {
                enumerable = CreateConverter(enumerable, property.ResolvedType);
            }

            var result = CreateResult(property);
            result.ResolveBuilder = builder;
            result.ResolveFailures = failures;
            result.Results = enumerable;
            return result;
        }

        private static IEnumerable CreateConverter(IEnumerable source, Type resolvedType)
            => (IEnumerable)Activator.CreateInstance(
                typeof(ResolveResultConverter<>).MakeGenericType(resolvedType),
                source
            );

        private static ResolveResultBase CreateResult(ResolvedPropertyTypeInfo propertyTypeInfo)
        {
            if (propertyTypeInfo.InterfaceType == typeof(IResolve<>))
            {
                return CreateResult(
                    typeof(ResolveResult<>),
                    propertyTypeInfo.ResolvedType
                );
            }

            if (propertyTypeInfo.InterfaceType == typeof(IResolveSingle<>))
            {
                return CreateResult(
                    typeof(ResolveResultSingle<>),
                    propertyTypeInfo.ResolvedType
                );
            }

            if (propertyTypeInfo.InterfaceType == typeof(IResolveSingleOrDefault<>))
            {
                return CreateResult(
                    typeof(ResolveResultSingleOrDefault<>),
                    propertyTypeInfo.ResolvedType
                );
            }

            // not reached
            throw ResolvedPropertyTypeInfo.InvalidInterfaceType(
                nameof(propertyTypeInfo),
                propertyTypeInfo.InterfaceType
            );
        }

        private static ResolveResultBase CreateResult(Type resultTypeDefinition, Type resolvedType)
            => (ResolveResultBase)Activator.CreateInstance(
                resultTypeDefinition.MakeGenericType(resolvedType)
            );

        private static IEnumerable<Type> GetEnumerableTypes(Type enumerableType)
            => from type in enumerableType.GetRuntimeBaseTypeChain()

               from interfaceType in type.ImplementedInterfaces
               where interfaceType.IsConstructedGenericType

               let interfaceTypeDefinition = interfaceType.GetGenericTypeDefinition()
               where interfaceTypeDefinition == typeof(IEnumerable<>)

               select interfaceType.GenericTypeArguments[0];
    }
}
