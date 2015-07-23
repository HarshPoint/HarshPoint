using System;

namespace HarshPoint.ObjectModel
{
    internal static class HarshObjectMetadataExceptionLoggerExtension
    {
        private static readonly HarshLogger SelfLog = HarshLog.ForContext(typeof(HarshObjectMetadataExceptionLoggerExtension));

        public static HarshObjectMetadataException ObjectMetadata(this HarshLoggerFatal logger, String format, params Object[] args)
        {
            if (logger == null)
            {
                throw SelfLog.Fatal.ArgumentNull(nameof(logger));
            }

            return logger.Write(
                Error.ObjectMetadataFormat(format, args)
            );
        }
    }
}
