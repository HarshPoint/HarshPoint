using System;

namespace HarshPoint.Provisioning
{
    public abstract class ProgressReport
    {
        public String Context { get; internal set; }
        public DateTimeOffset Timestamp { get; internal set; }
    }
}
