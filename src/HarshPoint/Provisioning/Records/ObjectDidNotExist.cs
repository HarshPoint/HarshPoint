namespace HarshPoint.Provisioning.Records
{
    public sealed class ObjectDidNotExist<T> : ObjectRecord<T>
    {
        public override HarshProvisionerRecordType RecordType 
            => HarshProvisionerRecordType.DoesNotExist;
    }
}
