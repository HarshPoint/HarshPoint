namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectAdded<T> : ObjectRecord<T>
    {
        public ObjectAdded(T @object)
            : base(@object)
        {
        }

        public override HarshProvisionerRecordType RecordType
            => HarshProvisionerRecordType.Added;
    }
}
