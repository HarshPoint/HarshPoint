using System;

namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectDidNotExist : IdentifiedRecord
    {
        public ObjectDidNotExist(String identifier)
            : base(identifier)
        {
            RecordType = HarshProvisionerRecordType.DidNotExist;
        }
    }
}
