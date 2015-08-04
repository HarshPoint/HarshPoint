using Serilog.Events;
using System;

namespace HarshPoint.Diagnostics
{
    public sealed class HarshMethodLogger
    {
        private readonly Object[] _args;
        private readonly LogEventLevel _level;
        private readonly HarshLogger _logger;
        private readonly String _methodName;

        internal HarshMethodLogger(HarshLogger logger, LogEventLevel level, String methodName, Object[] args)
        {
            if (logger == null)
            {
                throw SelfLog.Fatal.ArgumentNull(nameof(logger));
            }

            if (!Enum.IsDefined(typeof(LogEventLevel), level))
            {
                throw SelfLog.Fatal.InvalidEnumArgument(
                    nameof(level),
                    typeof(LogEventLevel),
                    level
                );
            }

            if (String.IsNullOrWhiteSpace(methodName))
            {
                throw SelfLog.Fatal.ArgumentNullOrWhiteSpace(nameof(methodName));
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

        public T Invoke<T>(Func<T> func)
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

        public void Invoke(Action action)
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

        private static readonly HarshLogger SelfLog = HarshLog.ForContext<HarshMethodLogger>();
    }

}
