using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class Resolvable
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(Resolvable));

        public static Type GetResolvedType(TypeInfo interfaceType)
        {
            if (interfaceType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(interfaceType));
            }

            if (interfaceType.IsGenericType)
            {
                var definition = interfaceType.GetGenericTypeDefinition();

                if (definition == typeof(IResolveOld<>) ||
                    definition == typeof(IResolve<>) ||
                    definition == typeof(IResolveSingle<>) ||
                    definition == typeof(IResolveSingleOrDefault<>))
                {
                    return interfaceType.GenericTypeArguments[0];
                }
            }

            return null;
        }
    }
}
