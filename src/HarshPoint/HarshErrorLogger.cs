using Serilog;
using Serilog.Events;
using System;

namespace HarshPoint
{
    public sealed class HarshErrorLogger
    {
        internal HarshErrorLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        public ILogger Logger { get; private set; }

        public ArgumentNullException ArgumentNull(String parameterName)
        {
            return Write(
                LogEventLevel.Fatal, 
                Error.ArgumentNull(parameterName)
            );
        }

        public ArgumentOutOfRangeException ArgumentEmptySequence(String parameterName)
        {
            return Write(
                LogEventLevel.Fatal,
                Error.ArgumentOutOfRange_EmptySequence(parameterName)
            );
        }

        public InvalidOperationException InvalidOperation(String message)
        {
            return Write(
                LogEventLevel.Fatal,
                Error.InvalidOperation(message)
            );
        }

        public TException Write<TException>(LogEventLevel level, TException exception)
            where TException : Exception
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Logger.Write(level, exception, "Exception thrown", new Object[0]);
            return exception;
        }

    }
}
