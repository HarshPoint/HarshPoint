using System;

namespace HarshPoint.Provisioning
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class ParameterAttribute : Attribute
    {
        private String _parameterSetName;

        public Boolean Mandatory
        {
            get;
            set;
        }

        public String ParameterSetName
        {
            get { return _parameterSetName; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    _parameterSetName = null;
                }
                else
                {
                    _parameterSetName = value;
                }
            }
        }
    }
}
