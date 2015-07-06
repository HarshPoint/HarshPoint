using Serilog.Events;
using System;

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
    }
}
