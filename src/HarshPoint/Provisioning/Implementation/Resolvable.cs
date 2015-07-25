using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    [Obsolete("Use ResolvedPropertyInfo")]
    internal static class Resolvable
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(Resolvable));

        public static Boolean IsResolveType(TypeInfo interfaceType)
            => GetResolvedType(interfaceType) != null;

        public static Type GetResolvedType(TypeInfo interfaceType)
            => ResolvedPropertyTypeInfo.TryParse(interfaceType)?.ResolvedType;
    }
}
