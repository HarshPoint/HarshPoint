using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class HarshProvisionerMetadataException : Exception
    {
        public HarshProvisionerMetadataException()
        {
        }

        public HarshProvisionerMetadataException(String message) 
            : base(message)
        {
        }

        public HarshProvisionerMetadataException(String message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
