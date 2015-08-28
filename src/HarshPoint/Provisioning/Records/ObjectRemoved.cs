namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectRemoved<T> : ObjectRecord<T>
    {
        public ObjectRemoved()
        {
        }

        public ObjectRemoved(T @object)
            : base(@object)
        {
        }

        public override HarshProvisionerRecordType RecordType
            => HarshProvisionerRecordType.Removed;
    }
}
