namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectAlreadyExists<T> : ObjectRecord<T>
    {
        public ObjectAlreadyExists(T @object)
            : base(@object)
        {
        }

        public override HarshProvisionerRecordType RecordType
            => HarshProvisionerRecordType.Exists;
    }
}
