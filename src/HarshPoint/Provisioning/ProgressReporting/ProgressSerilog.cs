using Serilog;
using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ProgressSerilog : IProgress<ProgressReport>
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

        public void Report(ProgressReport value)
        {
            _logger.Information("{Value}", value);
        }

        private static readonly HarshLogger SelfLog
            = HarshLog.ForContext<ProgressSerilog>();
    }
}
