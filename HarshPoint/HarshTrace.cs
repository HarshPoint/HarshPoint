using System;
using System.Collections.Immutable;
using System.Globalization;

namespace HarshPoint
{
    public static class HarshTrace
    {
        private static readonly Object SyncRoot = new Object();

        private static ImmutableList<HarshTraceListener> Listeners =
            ImmutableList.Create(
                HarshTraceListener.DebugListener
            );

        public static void AddListener(HarshTraceListener listener)
        {
            if (listener == null)
            {
                throw Error.ArgumentNull(nameof(listener));
            }

            lock (SyncRoot)
            {
                Listeners = Listeners.Add(listener);
            }
        }

        public static void RemoveListener(HarshTraceListener listener)
        {
            if (listener == null)
            {
                throw Error.ArgumentNull(nameof(listener));
            }

            lock (SyncRoot)
            {
                Listeners = Listeners.Remove(listener);
            }
        }

        public static void WriteError(Exception exception)
        {
            if (exception == null)
            {
                throw Error.ArgumentNull(nameof(exception));
            }

            WriteError(exception.ToString(), exception);
        }

        public static void WriteError(String message, Exception exception)
        {
            if (message == null)
            {
                throw Error.ArgumentNull(nameof(message));
            }

            if (exception == null)
            {
                throw Error.ArgumentNull(nameof(exception));
            }

            Write(new HarshTraceEvent(message, exception));
        }

        public static void WriteInfo(String message)
        {
            if (message == null)
            {
                throw Error.ArgumentNull(nameof(message));
            }

            Write(new HarshTraceEvent(message, null));
        }

        public static void WriteInfo(String format, params Object[] args)
        {
            WriteInfo(
                String.Format(CultureInfo.InvariantCulture, format, args)
            );
        }

        private static void Write(HarshTraceEvent e)
        {
            foreach (var listener in Listeners)
            {
                listener.Write(e);
            }
        }
    }
}
