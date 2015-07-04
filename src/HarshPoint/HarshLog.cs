using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint
{
    public static class HarshLog
    {
        public static HarshLogger ToHarshLogger(this ILogger logger)
        {
            if (logger == null)
            {
                throw Error.ArgumentNull(nameof(logger));
            }

            return (logger as HarshLogger) ?? new HarshLogger(logger);
        }

        public static HarshLogger ForContext(Type source)
            => Log.ForContext(source).ToHarshLogger();

        public static HarshLogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => Log.ForContext(enrichers.ToArray()).ToHarshLogger();

        public static HarshLogger ForContext(String propertyName, Object value, Boolean destructureObjects = false)
            => Log.ForContext(propertyName, value, destructureObjects).ToHarshLogger();

        public static HarshLogger ForContext<TSource>()
            => Log.ForContext<TSource>().ToHarshLogger();
    }
}
