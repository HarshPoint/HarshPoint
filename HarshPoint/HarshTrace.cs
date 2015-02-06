#define DEBUG

using System;
using System.Diagnostics;

namespace HarshPoint
{
    /// <summary>
    /// Until a better portable tracing solution comes by, use 
    /// <see cref="System.Diagnostics.Debug"/> even in Release build.
    /// </summary>
    internal static class HarshTrace
    {
        public static void WriteLine(String message)
        {
            Debug.WriteLine(message);
        }

        public static void WriteLine(String format, params Object[] args)
        {
            Debug.WriteLine(format, args);
        }
    }
}
