using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ResolveResultConverter
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultConverter));

        private static readonly ImmutableArray<Type> KnownPropertyTypeDefinitions =
            ImmutableArray.Create(typeof(IResolve<>), typeof(IResolveSingle<>), typeof(IResolveSingleOrDefault<>));

        public static Object CreateResult(Type propertyType, IEnumerable source)
        {
            if (propertyType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyType));
            }

            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            return CreateResult(propertyType.GetTypeInfo(), source);
        }

        public static Object CreateResult(TypeInfo propertyTypeInfo, IEnumerable source)
        {
            if (propertyTypeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyTypeInfo));
            }

            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            if (!propertyTypeInfo.IsGenericType)
            {
                throw Logger.Fatal.ArgumentOutOfRangeFormat(
                    nameof(propertyTypeInfo),
                    SR.ResolveResultConverter_PropertyTypeNotGeneric,
                    propertyTypeInfo
                );
            }

            var propertyTypeDefinition = propertyTypeInfo.GetGenericTypeDefinition();
            var resolvedType = propertyTypeInfo.GenericTypeArguments.First();

            if (propertyTypeDefinition == typeof(IResolve<>))
            {
                return Activator.CreateInstance(
                    typeof(ResolveResult<>).MakeGenericType(resolvedType),
                    source
                );
            }

            if (propertyTypeDefinition == typeof(IResolveSingle<>))
            {
                throw new NotImplementedException();
            }

            if (propertyTypeDefinition == typeof(IResolveSingleOrDefault<>))
            {
                throw new NotImplementedException();
            }

            throw Logger.Fatal.ArgumentOutOfRangeFormat(
                nameof(propertyTypeInfo),
                SR.ResolveResultConverter_PropertyTypeUnknownInterface,
                propertyTypeInfo,
                String.Join(", ", KnownPropertyTypeDefinitions.Select(t => t.Name))
            );
        }
    }
}
