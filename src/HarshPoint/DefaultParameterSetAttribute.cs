using System;

namespace HarshPoint
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DefaultParameterSetAttribute : Attribute
    {
        public DefaultParameterSetAttribute(String defaultParameterSetName)
        {
            if (defaultParameterSetName == null)
            {
                throw Error.ArgumentNull(nameof(defaultParameterSetName));
            }

            DefaultParameterSetName = defaultParameterSetName;
        }

        public String DefaultParameterSetName
        {
            get;
            private set;
        }
    }
}
