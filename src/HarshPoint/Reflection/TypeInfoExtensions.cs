using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class TypeInfoExtensions
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(TypeInfoExtensions));

        public static IEnumerable<TypeInfo> GetRuntimeBaseTypeChain(this Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            var typeInfo = type.GetTypeInfo();

            while (typeInfo != null)
            {
                yield return typeInfo;
                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<Type[]> ExtractGenericInterfaceTypeArguments(this Type type, Type interfaceTypeDefinition)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (interfaceTypeDefinition == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(interfaceTypeDefinition));
            }

            var interfaceTypeDefinitionInfo = interfaceTypeDefinition.GetTypeInfo();

            if (!interfaceTypeDefinitionInfo.IsGenericTypeDefinition ||
                !interfaceTypeDefinitionInfo.IsInterface)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(interfaceTypeDefinition),
                    SR.TypeInfoExtension_ArgNotInterfaceDefinition,
                    interfaceTypeDefinition
                );
            }

            if (type.IsConstructedGenericType &&
                type.GetGenericTypeDefinition() == interfaceTypeDefinition)
            {
                return ImmutableArray.Create(new[] { type.GenericTypeArguments });
            }

            return from t in type.GetRuntimeBaseTypeChain()

                   from it in t.ImplementedInterfaces
                   where it.IsConstructedGenericType

                   let itd = it.GetGenericTypeDefinition()
                   where itd == interfaceTypeDefinition

                   select it.GenericTypeArguments;

        }


        public static Boolean IsNullable(this TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            if (typeInfo.IsValueType)
            {
                return Nullable.GetUnderlyingType(typeInfo.AsType()) != null;
            }

            return true;
        }
    }
}
