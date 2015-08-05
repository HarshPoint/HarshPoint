using Serilog;

namespace HarshPoint.Provisioning.Output
{
    public sealed class HarshProvisionerOutputSinkSerilog : HarshProvisionerOutputSink
    {
        private readonly ILogger _logger;

        public HarshProvisionerOutputSinkSerilog(ILogger logger)
        {
            if (logger == null)
            {
                throw SelfLog.Fatal.ArgumentNull(nameof(logger));
            }

            _logger = logger;
        }

        protected internal override void WriteOutputCore(HarshProvisionerOutput output)
        {
            _logger.Information("{Output}", output);
        }

        private static readonly HarshLogger SelfLog = HarshLog.ForContext<HarshProvisionerOutputSinkSerilog>();
    }
}
