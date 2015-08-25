using System;

namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectAlreadyExists<T> : IdentifiedObjectRecord<T>
    {
        public ObjectAlreadyExists(String identifier, T @object)
            : base(identifier, @object)
        {
            RecordType = HarshProvisionerRecordType.AlreadyExists;
        }
    }
}
