using Serilog;
using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using HarshPoint.Diagnostics;

namespace HarshPoint
{
    public sealed class HarshLogger : ILogger
    {
        private readonly ILogger _inner;

        private HarshLoggerError _error;
        private HarshLoggerFatal _fatal;

        public HarshLogger(ILogger inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            _inner = inner;
        }

        public HarshLoggerError Error
            => HarshLazy.Initialize(ref _error, () => new HarshLoggerError(this));

        public HarshLoggerFatal Fatal
            => HarshLazy.Initialize(ref _fatal, () => new HarshLoggerFatal(this));

        public HarshMethodLogger Method(LogEventLevel level, String methodName, params Object[] args)
            => new HarshMethodLogger(this, level, methodName, args);

        public HarshMethodLogger Method(String methodName, params Object[] args)
            => Method(LogEventLevel.Debug, methodName, args);

        public void Debug(String messageTemplate, params Object[] propertyValues)
            => _inner.Debug(messageTemplate, propertyValues);

        public void Debug(Exception exception, String messageTemplate, params Object[] propertyValues)
            => _inner.Debug(exception, messageTemplate, propertyValues);

        public HarshLogger ForContext(Type source)
            => _inner.ForContext(source).ToHarshLogger();

        public HarshLogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
            => _inner.ForContext(enrichers).ToHarshLogger();

        public HarshLogger ForContext(params ILogEventEnricher[] enrichers)
            => _inner.ForContext(enrichers).ToHarshLogger();

        public HarshLogger ForContext(String propertyName, Object value)
            => _inner.ForContext(propertyName, value, destructureObjects: false).ToHarshLogger();

        public HarshLogger ForContext(String propertyName, Object value, Boolean destructureObjects)
            => _inner.ForContext(propertyName, value, destructureObjects).ToHarshLogger();

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
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
