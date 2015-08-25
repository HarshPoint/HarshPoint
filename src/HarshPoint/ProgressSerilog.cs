using Serilog;
using System;

namespace HarshPoint
{
    public sealed class ProgressSerilog<T> : IProgress<T>
    {
        private readonly ILogger _logger;

        public ProgressSerilog(ILogger logger)
        {
            if (logger == null)
            {
                throw SelfLog.Fatal.ArgumentNull(nameof(logger));
            }

            _logger = logger;
        }

        public void Report(T value)
        {
            _logger.Information("{Value}", value);
        }

        private static readonly HarshLogger SelfLog
            = HarshLog.ForContext(typeof(ProgressSerilog<>));
    }
}
