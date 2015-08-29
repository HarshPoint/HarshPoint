using System;
using static System.FormattableString;

namespace HarshPoint.Provisioning
{
    public abstract class HarshProvisionerRecord
    {
        public String Context { get; internal set; }

        public String Identifier { get; internal set; }

        public virtual Type ObjectType { get; } = null;

        public virtual HarshProvisionerRecordType RecordType { get; }
            = HarshProvisionerRecordType.Other;

        public DateTimeOffset Timestamp { get; internal set; }

        public override String ToString()
            => Invariant($"{RecordType} {Identifier}");
    }
}
