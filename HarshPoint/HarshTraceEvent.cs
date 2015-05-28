using System;
using System.Globalization;

namespace HarshPoint
{
    public sealed class HarshTraceEvent
    {
        public HarshTraceEvent(String message, Exception exception)
        {
            if (message == null)
            {
                throw Error.ArgumentNull(nameof(message));
            }

            Exception = exception;
            Message = message;
            TimeWritten = DateTimeOffset.Now;
        }

        public Exception Exception
        {
            get;
            private set;
        }

        public String Message
        {
            get;
            private set;
        }

        public DateTimeOffset TimeWritten
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0:yyyy-MM-dd HH:mm} {1}",
                TimeWritten,
                Message
            );
        }
    }
}
