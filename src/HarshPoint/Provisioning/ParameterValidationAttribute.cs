using System;
using System.Globalization;
using System.Reflection;

namespace HarshPoint.Provisioning
{
    public abstract class ParameterValidationAttribute : Attribute
    {

        protected String ParameterName
        {
            get;
            private set;
        }

        protected Type ParameterType
        {
            get;
            private set;
        }

        protected TypeInfo ParameterTypeInfo
        {
            get;
            private set;
        }
                
        public void Validate(PropertyInfo propertyInfo, Object value)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            ParameterName = propertyInfo.Name;
            ParameterType = propertyInfo.PropertyType;
            ParameterTypeInfo = ParameterType.GetTypeInfo();

            try
            {
                Validate(value);
            }
            finally
            {
                ParameterName = null;
                ParameterType = null;
                ParameterTypeInfo = null;
            }
        }

        protected abstract void Validate(Object value);

        protected Exception ValidationFailed(String format, params Object[] args)
        {
            return ValidationFailed(
                String.Format(CultureInfo.CurrentCulture, format, args)
            );
        }

        protected Exception ValidationFailed(String message)
        {
            return new ParameterValidationFailedException(
                ParameterName,
                message
            );
        }
    }
}
