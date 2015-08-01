using HarshPoint.Diagnostics;
using Serilog.Events;
using System;

namespace HarshPoint.ObjectModel
{
    internal static class ParameterValidationExceptionLoggerExtension
    {
        public static ParameterValidationException ParameterValidationFormat(this HarshLoggerError logger, Parameter parameter, String format, params Object[] args)
        {
            if (logger == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(logger));
            }

            if (parameter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameter));
            }

            return logger.Write(
                LogEventLevel.Error,
                new ParameterValidationException(
                    parameter.Name,
                    HarshLoggerWrapper.FormatCurrentCulture(format, args)
                )
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ParameterValidationExceptionLoggerExtension));
    }
}
