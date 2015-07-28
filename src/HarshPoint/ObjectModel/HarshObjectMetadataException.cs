using System;

namespace HarshPoint.ObjectModel
{
    public sealed class HarshObjectMetadataException : Exception
    {
        public HarshObjectMetadataException()
        {
        }

        public HarshObjectMetadataException(String message) 
            : base(message)
        {
        }

        public HarshObjectMetadataException(String message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
