using System;
using static System.FormattableString;

namespace HarshPoint.Provisioning.Records
{
    public abstract class ObjectRecord<T> : HarshProvisionerRecord
    {
        protected ObjectRecord() { }

        protected ObjectRecord(T @object)
        {
            Object = @object;
        }

        public T Object { get; }

        public Boolean ObjectAdded
            => RecordType == HarshProvisionerRecordType.Added;

        public Boolean ObjectRemoved
            => RecordType == HarshProvisionerRecordType.Removed;

        public override Type ObjectType => typeof(T);

        public override String ToString() 
            => Invariant($"{RecordType} {typeof(T).Name} {Identifier}");
    }
}
