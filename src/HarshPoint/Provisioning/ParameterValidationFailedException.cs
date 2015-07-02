using System;

namespace HarshPoint.Provisioning
{
    public class ParameterValidationFailedException : Exception
    {
        public ParameterValidationFailedException(String parameterName)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public ParameterValidationFailedException(String parameterName, String message) 
            : base(message)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public ParameterValidationFailedException(String parameterName, String message, Exception inner) 
            : base(message, inner)
        {
            if (String.IsNullOrWhiteSpace(parameterName))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(parameterName));
            }

            ParameterName = parameterName;
        }

        public String ParameterName { get; private set; }
    }
}
