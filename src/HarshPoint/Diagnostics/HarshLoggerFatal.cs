using Serilog.Events;
using System;
using System.Linq;

namespace HarshPoint.Diagnostics
{
    public sealed class HarshLoggerFatal : HarshLoggerWrapper
    {
        private static readonly HarshLogger SelfLogger = HarshLog.ForContext<HarshLoggerFatal>();

        internal HarshLoggerFatal(HarshLogger inner)
            : base(inner)
        {
        }

        public ArgumentException Argument(String parameterName, String message)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            if (message == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(message));
            }

            return Write(
                new ArgumentException(message, parameterName)
            );
        }

        public ArgumentException ArgumentFormat(String parameterName, String format, params Object[] args)
            => Argument(parameterName, FormatCurrentCulture(format, args));

        public ArgumentNullException ArgumentNull(String parameterName)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            return Write(
                new ArgumentNullException(parameterName)
            );
        }

        public ArgumentException ArgumentNullOrEmpty(String parameterName)
            => Argument(parameterName, SR.Error_ArgumentNullOrEmpty);

        public ArgumentException ArgumentNullOrWhitespace(String parameterName)
            => Argument(parameterName, SR.Error_ArgumentNullOrWhitespace);

        public ArgumentException ArgumentEmptySequence(String parameterName)
            => Argument(parameterName, SR.Error_SequenceEmpty);

        public ArgumentException ArgumentNotAssignableTo(String parameterName, Object value, params Type[] expectedBaseTypes)
        {
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
                return ArgumentFormat(
                    parameterName,
                    SR.Error_ObjectNotAssignableToOne,
                    value,
                    expectedBaseTypes[0]
                );
            }

            return ArgumentFormat(
                parameterName,
                SR.Error_ObjectNotAssignableToMany,
                value,
                String.Join(", ", expectedBaseTypes.Select(t => t.FullName))
            );
        }

        public ArgumentOutOfRangeException ArgumentOutOfRange(String parameterName, String message)
        {
            if (parameterName == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(parameterName));
            }

            if (message == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(message));
            }

            return Write(
                new ArgumentOutOfRangeException(parameterName, message)
            );
        }

        public ArgumentOutOfRangeException ArgumentOutOfRangeFormat(String parameterName, String format, params Object[] args)
            => ArgumentOutOfRangeFormat(parameterName, FormatCurrentCulture(format, args));

        public ArgumentException ArgumentTypeNotAssignableTo(String parameterName, Type type, Type expectedBaseType)
        {
            if (type == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(type));
            }

            if (expectedBaseType == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(expectedBaseType));
            }

            return ArgumentFormat(
                parameterName,
                SR.Error_TypeNotAssignableFrom,
                expectedBaseType.FullName,
                type.FullName
            );
        }

        public ArgumentOutOfRangeException InvalidEnumArgument(String parameterName, Type enumType, Object value)
            => ArgumentOutOfRangeFormat(parameterName, SR.Error_InvalidEnum, value, enumType);

        public InvalidOperationException InvalidOperation(String message)
        {
            if (message == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(message));
            }

            return Write(
                new InvalidOperationException(message)
            );
        }

        public InvalidOperationException InvalidOperationFormat(String format, params Object[] args)
            => InvalidOperation(FormatCurrentCulture(format, args));

        public NotImplementedException NotImplemented()
            => Write(new NotImplementedException());

        public InvalidOperationException PropertyNull(String propertyName)
        {
            if (propertyName == null)
            {
                SelfLogger.Fatal.ArgumentNull(nameof(propertyName));
            }

            return InvalidOperationFormat(SR.Error_PropertyNull, propertyName);
        }

        public TException Write<TException>(TException exception)
            where TException : Exception
            => Write(LogEventLevel.Fatal, exception);
    }
}
