using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedPropertyTypeInfo
    {
        public Type InterfaceType { get; private set; }

        public Type ResolvedType { get; private set; }

        public static ResolvedPropertyTypeInfo TryParse(TypeInfo propertyTypeInfo)
            => Parse(propertyTypeInfo, throwException: false);

        public static ResolvedPropertyTypeInfo Parse(TypeInfo propertyTypeInfo)
            => Parse(propertyTypeInfo, throwException: true);

        private static ResolvedPropertyTypeInfo Parse(TypeInfo propertyTypeInfo, Boolean throwException)
        {
            if (propertyTypeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyTypeInfo));
            }

            if (!propertyTypeInfo.IsGenericType)
            {
                if (!throwException)
                {
                    return null;
                }

                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(propertyTypeInfo),
                    SR.ResolveResultFactory_PropertyTypeNotGeneric,
                    propertyTypeInfo
                );
            }

            var interfaceType = propertyTypeInfo.GetGenericTypeDefinition();

            if (!KnownPropertyTypeDefinitions.Contains(interfaceType))
            {
                if (!throwException)
                {
                    return null;
                }

                throw InvalidInterfaceType(nameof(propertyTypeInfo), interfaceType);
            }

            return new ResolvedPropertyTypeInfo()
            {
                InterfaceType = interfaceType,
                ResolvedType = propertyTypeInfo.GenericTypeArguments.First(),
            };
        }

        public static Type GetResolvedType(TypeInfo propertyTypeInfo)
            => Parse(propertyTypeInfo).ResolvedType;

        public static Boolean IsResolveType(TypeInfo propertyTypeInfo)
            => GetResolvedType(propertyTypeInfo) != null;

        internal static Exception InvalidInterfaceType(String parameterName, Type interfaceType)
            => Logger.Fatal.ArgumentOutOfRangeFormat(
                parameterName,
                SR.ResolveResultFactory_PropertyTypeUnknownInterface,
                interfaceType,
                String.Join(", ", KnownPropertyTypeDefinitions.Select(t => t.Name))
            );

        private static readonly ImmutableArray<Type> KnownPropertyTypeDefinitions = ImmutableArray.Create(
            typeof(IResolve<>),
            typeof(IResolveSingle<>),
            typeof(IResolveSingleOrDefault<>)
        );

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolvedPropertyTypeInfo));
    }
}
