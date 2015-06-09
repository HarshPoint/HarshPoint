using System;

namespace HarshPoint.Provisioning
{
    public sealed class HarshProvisionerObjectResult<T, TIdentifier> : HarshProvisionerResult
    {
        public TIdentifier Identifier
        {
            get;
            set;
        }

        public T Object
        {
            get;
            set;
        }

        public Boolean ObjectAdded
        {
            get;
            set;
        }

        public Boolean ObjectRemoved
        {
            get;
            set;
        }

        public Boolean ObjectUpdated
        {
            get;
            set;
        }
    }
}