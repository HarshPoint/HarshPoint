using Serilog.Events;
using System;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class HarshProvisionerMetadataExceptionLoggerExtension
    {
        public static HarshProvisionerMetadataException ProvisionerMetadata(this HarshErrorLogger logger, String format, params Object[] args)
        {
            if (logger == null)
            {
                throw Error.ArgumentNull(nameof(logger));
            }

            return logger.Write(
                LogEventLevel.Fatal,
                Error.ProvisionerMetadataFormat(format, args)
            );
        }
    }
}
