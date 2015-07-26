using Serilog.Events;
using System;
using System.Globalization;

namespace HarshPoint
{
    public abstract class HarshLoggerWrapper
    {
        /// <summary>
        /// This <see cref="HarshLogger"/> is only used to report invalid arguments to this class.
        /// </summary>
        private static readonly HarshLogger SelfLogger = HarshLog.ForContext<HarshLoggerWrapper>();

        protected HarshLoggerWrapper(HarshLogger inner)
        {
            if (inner == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(inner));
            }

            InnerLogger = inner;
        }

        public TException Write<TException>(LogEventLevel level, TException exception)
            where TException : Exception
        {
            if (exception == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(exception));
            }

            InnerLogger.Write(
                level,
                exception,
                "Throwing exception",
                new Object[0]
            );

            return exception;
        }

        public HarshLogger InnerLogger { get; private set; }

        internal static String FormatCurrentCulture(String format, Object[] args)
        {
            if (format == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(format));
            }

            if (args == null)
            {
                throw SelfLogger.Fatal.ArgumentNull(nameof(args));
            }

            return String.Format(
                CultureInfo.CurrentUICulture,
                format,
                args
            );
        }
    }
}
