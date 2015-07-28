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
                throw Logger.Fatal.ArgumentNull(nameof(defaultParameterSetName));
            }

            DefaultParameterSetName = defaultParameterSetName;
        }

        public String DefaultParameterSetName
        {
            get;
            private set;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<DefaultParameterSetAttribute>();
    }
}
