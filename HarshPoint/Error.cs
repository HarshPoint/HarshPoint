using System;
using System.Globalization;
using System.Reflection;

namespace HarshPoint
{
    internal static class Error
    {
        public static Exception ArgumentNull(String paramName)
        {
            return new ArgumentNullException(paramName);
        }

        public static Exception ArgumentNullOrWhitespace(String paramName)
        {
            return ArgumentOutOfRange(
                paramName,
                SR.Error_ArgumentNullOrWhitespace
            );
        }

        public static Exception ArgumentOutOfRange(String paramName, String message)
        {
            return new ArgumentOutOfRangeException(paramName, message);
        }

        public static Exception ArgumentOutOfRangeFormat(String paramName, String format, params Object[] args)
        {
            return new ArgumentOutOfRangeException(paramName,
                Format(format, args)
            );
        }

        public static Exception ArgumentOutOfRange_ObjectNotAssignableTo(String paramName, TypeInfo baseType, Object shouldHaveBeenAssignable)
        {
            return ArgumentOutOfRangeFormat(
                paramName,
                SR.Error_ObjectNotAssignableTo,
                shouldHaveBeenAssignable,
                baseType
            );
        }

        public static Exception ArgumentOutOfRange_TypeNotAssignableFrom(String paramName, TypeInfo baseType, TypeInfo shouldHaveBeenAssignable)
        {
            return ArgumentOutOfRangeFormat(
                paramName,
                SR.Error_TypeNotAssignableFrom,
                baseType.FullName,
                shouldHaveBeenAssignable.FullName
            );
        }

        public static Exception InvalidOperation(String message)
        {
            return new InvalidOperationException(message);
        }

        public static Exception InvalidOperation(String format, params Object[] args)
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
