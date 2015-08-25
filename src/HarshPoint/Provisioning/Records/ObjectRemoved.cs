using System;

namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectRemoved : IdentifiedRecord
    {
        public ObjectRemoved(String identifier)
            : base(identifier)
        {
            RecordType = HarshProvisionerRecordType.Removed;
        }
    }
}
