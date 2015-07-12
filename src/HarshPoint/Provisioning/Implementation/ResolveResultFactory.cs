using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using HarshPoint.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ResolveResultFactory
    {
        private static readonly ImmutableArray<Type> KnownPropertyTypeDefinitions = ImmutableArray.Create(
            typeof(IResolve<>),
            typeof(IResolveSingle<>),
            typeof(IResolveSingleOrDefault<>)
        );

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultFactory));

        public static Object CreateResult(TypeInfo propertyTypeInfo, IEnumerable enumerable, IResolveBuilder builder)
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

            var parsed = ResolvedPropertyTypeInfo.Parse(propertyTypeInfo);

            var enumerableTypes = 
                GetEnumerableTypes(enumerable.GetType())
                .ToImmutableHashSet();

            if (!enumerableTypes.Contains(parsed.ResolvedType))
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(enumerable),
                    SR.ResolveResultFactory_ObjectNotConvertable,
                    enumerable,
                    typeof(IEnumerable<>).MakeGenericType(parsed.ResolvedType)
                );
            }

            var result = CreateResult(parsed);
            result.ResolveBuilder = builder;
            result.Results = enumerable;
            return result;
        }

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

            throw Logger.Fatal.ArgumentOutOfRangeFormat(
                nameof(propertyTypeInfo),
                SR.ResolveResultFactory_PropertyTypeUnknownInterface,
                propertyTypeInfo.InterfaceType,
                String.Join(", ", KnownPropertyTypeDefinitions.Select(t => t.Name))
            );
        }

        private static ResolveResultBase CreateResult(Type resultTypeDefinition, Type resolvedType)
        {
            return (ResolveResultBase)Activator.CreateInstance(
                resultTypeDefinition.MakeGenericType(resolvedType)
            );
        }

        private static IEnumerable<Type> GetEnumerableTypes(Type enumerableType)
        {
            return from type in enumerableType.GetRuntimeBaseTypeChain()

                   from interfaceType in type.ImplementedInterfaces
                   where interfaceType.IsConstructedGenericType

                   let interfaceTypeDefinition = interfaceType.GetGenericTypeDefinition()
                   where interfaceTypeDefinition == typeof(IEnumerable<>)

                   select interfaceType.GenericTypeArguments[0];
        }

        private struct ResolvedPropertyTypeInfo
        {
            public Type InterfaceType
            {
                get;
                private set;
            }

            public Type ResolvedType
            {
                get;
                private set;
            }
            
            public static ResolvedPropertyTypeInfo Parse(TypeInfo propertyTypeInfo)
            {
                if (propertyTypeInfo == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(propertyTypeInfo));
                }

                if (!propertyTypeInfo.IsGenericType)
                {
                    throw Logger.Fatal.ArgumentOutOfRangeFormat(
                        nameof(propertyTypeInfo),
                        SR.ResolveResultFactory_PropertyTypeNotGeneric,
                        propertyTypeInfo
                    );
                }

                var interfaceType = propertyTypeInfo.GetGenericTypeDefinition();

                if (!KnownPropertyTypeDefinitions.Contains(interfaceType))
                {
                    throw Logger.Fatal.ArgumentOutOfRangeFormat(
                        nameof(propertyTypeInfo),
                        SR.ResolveResultFactory_PropertyTypeUnknownInterface,
                        propertyTypeInfo,
                        String.Join(", ", KnownPropertyTypeDefinitions.Select(t => t.Name))
                    );
                }

                return new ResolvedPropertyTypeInfo()
                {
                    InterfaceType = interfaceType,
                    ResolvedType = propertyTypeInfo.GenericTypeArguments.First(),
                };
            }
        }
    }
}
