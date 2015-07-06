using Serilog;
using Serilog.Events;
using System;

namespace HarshPoint
{
    public sealed class HarshLoggerError : HarshLoggerWrapper
    {
        public HarshLoggerError(HarshLogger inner)
            : base(inner)
        {
        }

        public TException Write<TException>(TException exception)
            where TException : Exception
        {
            return Write(LogEventLevel.Error, exception);
        }
    }
}
