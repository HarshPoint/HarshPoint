using Microsoft.SharePoint.Client;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ContextStateResolver
    {
        public static Object Create(Type resolvedType)
        {
            if (resolvedType == null)
            {
                throw Error.ArgumentNull(nameof(resolvedType));
            }

            if (ClientObjectTypeInfo.IsAssignableFrom(resolvedType.GetTypeInfo()))
            {
                return Activator.CreateInstance(
                    typeof(ClientObjectContextStateResolver<>).MakeGenericType(resolvedType)
                );
            }

            return Activator.CreateInstance(
                typeof(ContextStateResolver<>).MakeGenericType(resolvedType)
            );
        }

        private static readonly TypeInfo ClientObjectTypeInfo =
            typeof(ClientObject).GetTypeInfo();
    }
}
