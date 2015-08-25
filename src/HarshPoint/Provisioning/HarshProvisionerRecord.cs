using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning
{
    public abstract class HarshProvisionerRecord
    {
        public String Context { get; internal set; }
        public HarshProvisionerRecordType RecordType { get; internal set; }
        public DateTimeOffset Timestamp { get; internal set; }
    }
}
