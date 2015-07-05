using HarshPoint.Provisioning.Implementation;
using Serilog.Events;
using System;

namespace HarshPoint.Provisioning
{
    internal static class ParameterValidationExceptionLoggerExtension
    {
        public static ParameterValidationException ParameterValidationFormat(this HarshErrorLogger logger, Parameter parameter, String format, params Object[] args)
        {
            if (logger == null)
            {
                throw Error.ArgumentNull(nameof(logger));
            }

            if (parameter == null)
            {
                throw Error.ArgumentNull(nameof(parameter));
            }

            return logger.Write(
                LogEventLevel.Error,
                Error.ParameterValidationFormat(parameter.Name, format, args)
            );
        }
    }
}
