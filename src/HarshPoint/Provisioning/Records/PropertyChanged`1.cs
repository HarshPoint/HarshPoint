using System;
using static System.FormattableString;

namespace HarshPoint.Provisioning.Records
{
    public sealed class PropertyChanged<T> : PropertyRecord<T>
    {
        public PropertyChanged(
            T @object, 
            Object oldValue, 
            Object newValue
        )
            : base(@object)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override String ToString()
            => Invariant($"{Identifier} '{OldValue}' => '{NewValue}'");

        public override HarshProvisionerRecordType RecordType
            => HarshProvisionerRecordType.Changed;

        public Object NewValue { get; }
        public Object OldValue { get; }
    }
}
