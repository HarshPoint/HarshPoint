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

        public MethodCallLogger MethodCall(LogEventLevel level, String methodName, params Object[] args)
            => new MethodCallLogger(this, level, methodName, args);

        public MethodCallLogger MethodCall(String methodName, params Object[] args)
            => MethodCall(LogEventLevel.Debug, methodName, args);

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

        public struct MethodCallLogger
        {
            private readonly Object[] _args;
            private readonly LogEventLevel _level;
            private readonly HarshLogger _logger;
            private readonly String _methodName;

            public MethodCallLogger(HarshLogger logger, LogEventLevel level, String methodName, Object[] args)
            {
                if (logger == null)
                {
                    throw SelfLog.Fatal.ArgumentNull(nameof(logger));
                }

                if (Enum.IsDefined(typeof(LogEventLevel), level))
                {
                    throw SelfLog.Fatal.InvalidEnumArgument(
                        nameof(level),
                        typeof(LogEventLevel),
                        level
                    );
                }

                if (String.IsNullOrWhiteSpace(methodName))
                {
                    throw SelfLog.Fatal.ArgumentNullOrWhitespace(nameof(methodName));
                }

                if (args == null)
                {
                    throw SelfLog.Fatal.ArgumentNull(nameof(args));
                }

                _logger = logger;
                _level = level;
                _methodName = methodName;
                _args = args;
            }

            public T Call<T>(Func<T> func)
            {
                if (func == null)
                {
                    throw SelfLog.Fatal.ArgumentNull(nameof(func));
                }

                Enter();
                var result = func();
                Leave(result);
                return result;
            }

            public void Call(Action action)
            {
                if (action == null)
                {
                    throw SelfLog.Fatal.ArgumentNull(nameof(action));
                }

                Enter();
                action();
                Leave();
            }

            private void Enter()
                => _logger.Write(_level, "{Method:l} called with {@Arguments}", _methodName, _args);

            private void Leave()
                => _logger.Write(_level, "{Method:l} returned.");

            private void Leave(Object result)
                => _logger.Write(_level, "{Method:l} returned {ReturnValue}", result);

            private static readonly HarshLogger SelfLog = HarshLog.ForContext<MethodCallLogger>();
        }
    }
}
