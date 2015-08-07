using HarshPoint.ObjectModel;
using HarshPoint.Reflection;
using System;
using System.Collections;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ProvisioningDefaultValuePolicy : IDefaultValuePolicy
    {
        private ProvisioningDefaultValuePolicy() { }

        public Boolean IsDefaultValue(Object value)
        {
            if (value == null)
            {
                return true;
            }

            if (Guid.Empty.Equals(value))
            {
                return true;
            }

            var str = (value as String);
            if (str != null)
            {
                return String.IsNullOrWhiteSpace(str);
            }

            var enumerable = (value as IEnumerable);
            if (enumerable != null)
            {
                return !enumerable.Any();
            }

            return false;
        }

        public Boolean SupportsType(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            if (typeInfo.IsNullable())
            {
                return true;
            }

            if (GuidTypeInfo.Equals(typeInfo))
            {
                return true;
            }

            if (IEnumerableTypeInfo.IsAssignableFrom(typeInfo))
            {
                return true;
            }

            return false;
        }

        private static readonly TypeInfo GuidTypeInfo = typeof(Guid).GetTypeInfo();

        private static readonly TypeInfo IEnumerableTypeInfo = typeof(IEnumerable).GetTypeInfo();

        private static readonly HarshLogger Logger = HarshLog.ForContext<ProvisioningDefaultValuePolicy>();

        public static IDefaultValuePolicy Instance { get; } = new ProvisioningDefaultValuePolicy();
    }
}
