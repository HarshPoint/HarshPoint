using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        public static Type GetResolvedType(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw Error.ArgumentNull(nameof(interfaceType));
            }

            var info = interfaceType.GetTypeInfo();

            if (info.IsGenericType)
            {
                var definition = info.GetGenericTypeDefinition();

                if (definition == typeof(IResolve<>))
                {
                    return info.GenericTypeArguments[0];
                }
            }

            return null;
        }
    }
}
