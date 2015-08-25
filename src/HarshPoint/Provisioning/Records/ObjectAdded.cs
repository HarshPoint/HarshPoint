using System;

namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectAdded<T> : IdentifiedObjectRecord<T>
    {
        public ObjectAdded(String identifier, T @object)
            : base(identifier, @object)
        {
            RecordType = HarshProvisionerRecordType.Added;
        }
    }
}
