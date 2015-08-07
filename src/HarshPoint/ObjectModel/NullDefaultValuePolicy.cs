using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public sealed class NullDefaultValuePolicy : IDefaultValuePolicy
    {
        public Boolean IsDefaultValue(Object value)
            => ReferenceEquals(value, null);

        public Boolean SupportsType(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            return typeInfo.IsNullable();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<NullDefaultValuePolicy>();
    }
}
