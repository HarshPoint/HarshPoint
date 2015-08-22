using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ObjectDidNotExist : IdentifiedProgressReportBase
    {
        public ObjectDidNotExist(String identifier, Object parent)
            : base(identifier, parent)
        {
        }
    }
}
