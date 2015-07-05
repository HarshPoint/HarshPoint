using System;

namespace HarshPoint.Provisioning
{
    public class ParameterValidationException : Exception
    {
        public ParameterValidationException(String parameterName)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentOutOfRange_NullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public ParameterValidationException(String parameterName, String message) 
            : base(message)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentOutOfRange_NullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public ParameterValidationException(String parameterName, String message, Exception inner) 
            : base(message, inner)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentOutOfRange_NullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public String ParameterName { get; private set; }
    }
}
