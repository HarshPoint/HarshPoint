using System;
using static System.FormattableString;

namespace HarshPoint.Provisioning.Records
{
    public sealed class PropertyUnchanged<T> : PropertyRecord<T>
    {
        public PropertyUnchanged(
            T @object, 
            Object oldValue
        )
            : base(@object)
        {
            OldValue = oldValue;
        }

        public override String ToString()
            => Invariant($"{Identifier} = '{OldValue}'");

        public Object OldValue { get; }

        public override HarshProvisionerRecordType RecordType
            => HarshProvisionerRecordType.Unchanged;
    }
}
