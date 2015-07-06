using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HarshPoint
{
    //[Obsolete("Use Logger.Fatal/Logger.Error")]
    internal static class Error
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(Error));

        public static ArgumentNullException ArgumentNull(String paramName)
        {
            if (paramName == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(paramName));
            }

            return new ArgumentNullException(paramName);
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRange(String paramName, String message)
        {
            if (paramName == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(paramName));
            }

            if (message == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(message));
            }

            return new ArgumentOutOfRangeException(paramName, message);
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRangeFormat(String paramName, String format, params Object[] args)
        {
            return new ArgumentOutOfRangeException(
                paramName,
                Format(format, args)
            );
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRange_EmptySequence(String paramName)
        {
            return ArgumentOutOfRange(paramName, SR.Error_SequenceEmpty);
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRange_NullOrWhitespace(String paramName)
        {
            return ArgumentOutOfRange(
                paramName,
                SR.Error_ArgumentNullOrWhitespace
            );
        }


        public static ArgumentOutOfRangeException ArgumentOutOfRange_TypeNotAssignableFrom(String paramName, TypeInfo baseType, TypeInfo shouldHaveBeenAssignable)
        {
            return ArgumentOutOfRangeFormat(
                paramName,
                SR.Error_TypeNotAssignableFrom,
                baseType.FullName,
                shouldHaveBeenAssignable.FullName
            );
        }

        public static ParameterValidationException ParameterValidationFormat(String parameterName, String format, params Object[] args)
            => new ParameterValidationException(
                parameterName,
                Format(format, args)
            );

        public static HarshProvisionerMetadataException ProvisionerMetadataFormat(String format, params Object[] args)
        {
            return new HarshProvisionerMetadataException(
                Format(format, args)
            );
        }

        public static InvalidOperationException InvalidOperation(String message)
        {
            return new InvalidOperationException(message);
        }

        public static InvalidOperationException InvalidOperationFormat(String format, params Object[] args)
        {
            return new InvalidOperationException(
                Format(format, args)
            );
        }

        private static String Format(String format, Object[] args)
        {
            if (format == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(format));
            }

            if (args == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(args));
            }

            return String.Format(
                CultureInfo.CurrentUICulture,
                format,
                args
            );
        }
    }
}
