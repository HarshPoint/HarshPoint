using Serilog;
using System;
using System.Diagnostics;

namespace HarshPoint.Tests
{
    internal sealed class SerilogTraceListener : TraceListener
    {
        public override void Write(String message)
        {
            WriteLine(message);
        }

        public override void WriteLine(String message)
        {
            //Log.Information("{$TraceListenerMessage}", message);
        }
    }
}
