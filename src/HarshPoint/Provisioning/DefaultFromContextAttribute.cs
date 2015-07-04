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
            TagType = tagType;
        }

        public Type TagType
        {
            get;
            private set;
        }
    }
}
