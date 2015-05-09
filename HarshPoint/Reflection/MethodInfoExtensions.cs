using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class MethodInfoExtensions
    {
        public static IEnumerable<MethodInfo> GetRuntimeBaseMethodChain(this MethodInfo methodInfo)
        {
            return GetRuntimeBaseMethodChain(methodInfo, null);
        }

        public static IEnumerable<MethodInfo> GetRuntimeBaseMethodChain(this MethodInfo methodInfo, Type subtype)
        {
            if (methodInfo == null)
            {
                throw Error.ArgumentNull(nameof(methodInfo));
            }

            if (subtype == null)
            {
                subtype = methodInfo.DeclaringType;
            }

            var typeInfo = methodInfo.DeclaringType.GetTypeInfo();
            var subtypeInfo = subtype.GetTypeInfo();

            if (!typeInfo.IsAssignableFrom(subtypeInfo))
            {
                throw Error.ArgumentOutOfRange_TypeNotAssignableFrom(
                    nameof(subtype),
                    typeInfo,
                    subtypeInfo
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
        {
            return GetRuntimeBaseMethodChain(methodInfo, subtype).FirstOrDefault();
        }
    }
}
