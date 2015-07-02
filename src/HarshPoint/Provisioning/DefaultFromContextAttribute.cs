using System;
using System.Reflection;

namespace HarshPoint.Provisioning
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DefaultFromContextAttribute : Attribute
    {
        public DefaultFromContextAttribute()
        {
        }

        public DefaultFromContextAttribute(Type tagType)
        {
            if (tagType == null)
            {
                return;
            }

            var info = tagType.GetTypeInfo();

            if (!IDefaultFromContextTagTypeInfo.IsAssignableFrom(info))
            {
                throw Error.ArgumentOutOfRange_TypeNotAssignableFrom(
                    nameof(tagType),
                    IDefaultFromContextTagTypeInfo,
                    info
                );
            }

            TagType = tagType;
        }

        public Type TagType
        {
            get;
            private set;
        }

        private static readonly TypeInfo IDefaultFromContextTagTypeInfo = typeof(IDefaultFromContextTag).GetTypeInfo();
    }
}
