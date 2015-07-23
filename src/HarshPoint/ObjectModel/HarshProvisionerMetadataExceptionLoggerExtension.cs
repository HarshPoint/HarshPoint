using System;

namespace HarshPoint.ObjectModel
{
    internal static class HarshProvisionerMetadataExceptionLoggerExtension
    {
        private static readonly HarshLogger SelfLog = HarshLog.ForContext(typeof(HarshProvisionerMetadataExceptionLoggerExtension));

        public static HarshProvisionerMetadataException ProvisionerMetadata(this HarshLoggerFatal logger, String format, params Object[] args)
        {
            if (logger == null)
            {
                throw SelfLog.Fatal.ArgumentNull(nameof(logger));
            }

            return logger.Write(
                Error.ProvisionerMetadataFormat(format, args)
            );
        }
    }
}
