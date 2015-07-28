using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint
{
    public static class HarshLog
    {
        public static HarshLogger ToHarshLogger(this ILogger logger)
        {
            if (logger == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(logger));
            }

            return (logger as HarshLogger) ?? new HarshLogger(logger);
        }

        public static HarshLogger ForContext(Type source)
            => Log.ForContext(source).ToHarshLogger();

        public static HarshLogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => Log.ForContext(enrichers.ToArray()).ToHarshLogger();

        public static HarshLogger ForContext(String propertyName, Object value)
            => Log.ForContext(propertyName, value, destructureObjects: false).ToHarshLogger();

        public static HarshLogger ForContext(String propertyName, Object value, Boolean destructureObjects)
            => Log.ForContext(propertyName, value, destructureObjects).ToHarshLogger();

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static HarshLogger ForContext<TSource>()
            => Log.ForContext<TSource>().ToHarshLogger();

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshLog));
    }
}
