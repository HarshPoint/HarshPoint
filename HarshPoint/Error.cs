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

        public static Exception InvalidOperation(String message)
        {
            return new Exception(message);
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
