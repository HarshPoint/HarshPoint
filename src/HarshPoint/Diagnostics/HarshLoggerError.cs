using Serilog.Events;
using System;

namespace HarshPoint.Diagnostics
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
