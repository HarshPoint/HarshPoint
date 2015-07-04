using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace HarshPoint
{
    internal sealed class HarshLogger : ILogger
    {
        private readonly ILogger _inner;
        private readonly HarshErrorLogger _error;

        public HarshLogger(ILogger inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            _inner = inner;
            _error = new HarshErrorLogger(this);
        }

        public HarshErrorLogger Error => _error;

        public void Debug(String messageTemplate, params Object[] propertyValues)
            => _inner.Debug(messageTemplate, propertyValues);

        public void Debug(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Debug(exception, messageTemplate, propertyValues);

        public HarshLogger ForContext(Type source)
            => _inner.ForContext(source).ToHarshLogger();

        public HarshLogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => _inner.ForContext(enrichers).ToHarshLogger();

        public HarshLogger ForContext(String propertyName, Object value, Boolean destructureObjects = false)
            => _inner.ForContext(propertyName, value, destructureObjects).ToHarshLogger();

        public HarshLogger ForContext<TSource>()
            => _inner.ForContext<TSource>().ToHarshLogger();

        public void Information(String messageTemplate, params Object[] propertyValues)
            => _inner.Information(messageTemplate, propertyValues);

        public void Information(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Information(exception, messageTemplate, propertyValues);

        public Boolean IsEnabled(LogEventLevel level)
            => _inner.IsEnabled(level);

        public void Verbose(String messageTemplate, params Object[] propertyValues)
            => _inner.Verbose(messageTemplate, propertyValues);

        public void Verbose(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Verbose(exception, messageTemplate, propertyValues);

        public void Warning(String messageTemplate, params Object[] propertyValues)
            => _inner.Warning(messageTemplate, propertyValues);

        public void Warning(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Warning(exception, messageTemplate, propertyValues);

        public void Write(LogEvent logEvent)
            => _inner.Write(logEvent);

        public void Write(LogEventLevel level, String messageTemplate, params Object[] propertyValues)
            => _inner.Write(level, messageTemplate, propertyValues);

        public void Write(LogEventLevel level, Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Write(level, exception, messageTemplate, propertyValues);

        void ILogger.Error(String messageTemplate, params Object[] propertyValues)
            => _inner.Error(messageTemplate, propertyValues);

        void ILogger.Error(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Error(exception, messageTemplate, propertyValues);

        void ILogger.Fatal(String messageTemplate, params Object[] propertyValues)
            => _inner.Fatal(messageTemplate, propertyValues);

        void ILogger.Fatal(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Fatal(exception, messageTemplate, propertyValues);

        ILogger ILogger.ForContext(Type source)
            => ForContext(source);

        ILogger ILogger.ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => ForContext(enrichers);

        ILogger ILogger.ForContext(String propertyName, Object value, Boolean destructureObjects)
            => ForContext(propertyName, value, destructureObjects);

        ILogger ILogger.ForContext<TSource>()
            => ForContext<TSource>();
    }
}
