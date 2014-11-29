using System;
using System.Globalization;

namespace HarshPoint
{
    internal static class Error
    {
        public static Exception ArgumentNull(String paramName)
        {
            return new ArgumentNullException(paramName);
        }

        public static Exception ArgumentOutOfRange(String paramName, String message)
        {
            return new ArgumentOutOfRangeException(paramName, message);
        }

        public static Exception ArgumentOutOfRange(String paramName, String format, params Object[] args)
        {
            return new ArgumentOutOfRangeException(paramName,
                Format(format, args)
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
