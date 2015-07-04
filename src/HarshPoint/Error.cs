using HarshPoint.Provisioning.Implementation;
using System;
using System.Globalization;
using System.Reflection;

namespace HarshPoint
{
    internal static class Error
    {
        public static ArgumentNullException ArgumentNull(String paramName)
        {
            return new ArgumentNullException(paramName);
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRange(String paramName, String message)
        {
            return new ArgumentOutOfRangeException(paramName, message);
        }

        public static ArgumentOutOfRangeException ArgumentOutOfRangeFormat(String paramName, String format, params Object[] args)
        {
            return new ArgumentOutOfRangeException(paramName,
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

        public static ArgumentOutOfRangeException ArgumentOutOfRange_ObjectNotAssignableTo(String paramName, TypeInfo baseType, Object shouldHaveBeenAssignable)
        {
            return ArgumentOutOfRangeFormat(
                paramName,
                SR.Error_ObjectNotAssignableTo,
                shouldHaveBeenAssignable,
                baseType
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
            return String.Format(
                CultureInfo.CurrentUICulture,
                format,
                args
            );
        }
    }
}
