using System;

namespace HarshPoint.ObjectModel
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
