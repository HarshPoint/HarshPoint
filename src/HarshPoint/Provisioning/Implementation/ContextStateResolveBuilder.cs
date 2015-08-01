using Microsoft.SharePoint.Client;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ContextStateResolveBuilder
    {
        public static Object Create(Type resolvedType)
        {
            if (resolvedType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvedType));
            }

            if (ClientObjectTypeInfo.IsAssignableFrom(resolvedType.GetTypeInfo()))
            {
                return Activator.CreateInstance(
                    typeof(ClientObjectContextStateResolveBuilder<>).MakeGenericType(resolvedType)
                );
            }

            return Activator.CreateInstance(
                typeof(ContextStateResolveBuilder<>).MakeGenericType(resolvedType)
            );
        }

        private static readonly TypeInfo ClientObjectTypeInfo =
            typeof(ClientObject).GetTypeInfo();

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ContextStateResolveBuilder));
    }
}
