using Serilog.Events;
using System;
using System.Linq;
using System.Reflection;

namespace HarshPoint
{
    public sealed class HarshLoggerFatal : HarshLoggerWrapper
    {
        private static readonly HarshLogger SelfLogger = HarshLog.ForContext<HarshLoggerFatal>();

        internal HarshLoggerFatal(HarshLogger inner)
            : base(inner)
        {
        }

        public ArgumentNullException ArgumentNull(String parameterName)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            return Write(
                Error.ArgumentNull(parameterName)
            );
        }

        public ArgumentOutOfRangeException ArgumentEmptySequence(String parameterName)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            return Write(
                Error.ArgumentOutOfRange_EmptySequence(parameterName)
            );
        }

        public ArgumentOutOfRangeException ArgumentNotAssignableTo(String parameterName, Object value, params Type[] expectedBaseTypes)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            if (value == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(value));
            }

            if (expectedBaseTypes == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(expectedBaseTypes));
            }

            if (!expectedBaseTypes.Any())
            {
                throw SelfLogger.Fatal.ArgumentEmptySequence(nameof(expectedBaseTypes));
            }

            if (expectedBaseTypes.Length == 1)
            {
                return Write(
                    Error.ArgumentOutOfRangeFormat(
                        parameterName,
                        SR.Error_ObjectNotAssignableToOne,
                        value,
                        expectedBaseTypes
                    )
                );
            }

            return Write(
                Error.ArgumentOutOfRangeFormat(
                    parameterName,
                    SR.Error_ObjectNotAssignableToMany,
                    value,
                    String.Join(", ", expectedBaseTypes.Select(t => t.FullName))
                )
            );
        }

        public InvalidOperationException InvalidOperation(String message)
        {
            if (message == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(message));
            }

            return Write(
                Error.InvalidOperation(message)
            );
        }

        public TException Write<TException>(TException exception)
            where TException : Exception
        {
            return Write(LogEventLevel.Fatal, exception);
        }

    }
}
