#define DEBUG

using System;
using System.Diagnostics;
using System.Globalization;

namespace HarshPoint
{
    /// <summary>
    /// Until a better portable tracing solution comes by, use 
    /// <see cref="System.Diagnostics.Debug"/> even in Release build.
    /// </summary>
    public static class HarshTrace
    {
        private static readonly Object _syncRoot = new Object();

        private static EventHandler<HarshTraceEventArgs> _onLineWritten =
            (sender, args) => Debug.WriteLine(args.Message);

        public static event EventHandler<HarshTraceEventArgs> LineWritten
        {
            add { lock (_syncRoot) _onLineWritten += value; }
            remove { lock (_syncRoot) _onLineWritten -= value; }
        }

        public static void WriteLine(String message)
        {
            _onLineWritten(null, new HarshTraceEventArgs(message));
        }

        public static void WriteLine(String format, params Object[] args)
        {
            WriteLine(
                String.Format(CultureInfo.InvariantCulture, format, args)
            );
        }
    }

    public sealed class HarshTraceEventArgs : EventArgs
    {
        public HarshTraceEventArgs(String message)
        {
            if (message == null)
            {
                throw Error.ArgumentNull(nameof(message));
            }

            Message = message;
        }
        public String Message
        {
            get;
            private set;
        }
    }
}
