using System;
using System.IO;
using System.Threading.Tasks;

namespace HarshPoint
{
    public abstract class HarshTraceListener
    {
        public abstract Task Write(HarshTraceEvent traceEvent);

        public static HarshTraceListener FromTextWriter(TextWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            return new HarshTextWriterTraceListener(writer);
        }

        internal static HarshTraceListener DebugListener = new HarshDebugTraceListener();
    }
}
