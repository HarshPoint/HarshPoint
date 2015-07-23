﻿using System;
using System.Collections.Generic;
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
