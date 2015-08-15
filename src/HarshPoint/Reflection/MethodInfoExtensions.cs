using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class MethodInfoExtensions
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(MethodInfoExtensions));

        public static IEnumerable<MethodInfo> GetRuntimeBaseMethodChain(this MethodInfo methodInfo)
            => GetRuntimeBaseMethodChain(methodInfo, null);

        public static IEnumerable<MethodInfo> GetRuntimeBaseMethodChain(this MethodInfo methodInfo, Type subtype)
        {
            if (methodInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(methodInfo));
            }

            if (subtype == null)
            {
                subtype = methodInfo.DeclaringType;
            }

            var typeInfo = methodInfo.DeclaringType.GetTypeInfo();
            var subtypeInfo = subtype.GetTypeInfo();

            if (!typeInfo.IsAssignableFrom(subtypeInfo))
            {
                throw Logger.Fatal.ArgumentTypeNotAssignableTo(
                    nameof(subtype),
                    subtype,
                    methodInfo.DeclaringType
                );
            }

            var baseMethodInfo = methodInfo.GetRuntimeBaseDefinition();
            var current = subtypeInfo;

            while (current != null)
            {
                var overridenHere = current.DeclaredMethods.FirstOrDefault(
                    m => m.GetRuntimeBaseDefinition() == baseMethodInfo
                );

                if (overridenHere != null)
                {
                    yield return overridenHere;
                }

                current = current.BaseType?.GetTypeInfo();
            }
        }

        public static MethodInfo GetOverrideIn(this MethodInfo methodInfo, Type subtype)
            => GetRuntimeBaseMethodChain(methodInfo, subtype).FirstOrDefault();
    }
}
