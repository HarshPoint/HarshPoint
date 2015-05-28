using System;
using System.IO;
using System.Threading.Tasks;

namespace HarshPoint
{
    internal sealed class HarshTextWriterTraceListener : HarshTraceListener
    {
        private readonly TextWriter _writer;

        public HarshTextWriterTraceListener(TextWriter writer)
        {
            _writer = writer;
        }

        public override Task Write(HarshTraceEvent e)
        {
            if (e == null)
            {
                throw Error.ArgumentNull(nameof(e));
            }

            return _writer.WriteLineAsync(e.ToString());
        }
    }
}
