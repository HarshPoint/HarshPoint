using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MandatoryAttribute : ParameterValidationAttribute
    {
        protected override void Validate(Object value)
        {
            if (value == null)
            {
                throw ValidationFailed("{0} cannot be null.", ParameterName);
            }

            if (ParameterType == typeof(String))
            {
                var str = (String)(value);

                if (String.IsNullOrWhiteSpace(str))
                {
                    throw ValidationFailed("{0} cannot be an empty or whitespace string.", ParameterName);
                }
            }
            else if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(ParameterTypeInfo))
            {
                var collection = (IEnumerable)(value);

                if (!collection.Cast<Object>().Any())
                {
                    throw ValidationFailed("{0} cannot be an empty collection.", ParameterName);
                }
            }
            else if (ParameterTypeInfo.IsValueType)
            {
                if (Equals(Activator.CreateInstance(ParameterType), value))
                {
                    throw ValidationFailed("{0} cannot be an empty {1}.", ParameterName, ParameterType);
                }
            }
        }
    }
}
