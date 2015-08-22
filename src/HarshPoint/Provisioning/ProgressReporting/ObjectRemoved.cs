using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ObjectRemoved : IdentifiedProgressReportBase
    {
        public ObjectRemoved(String identifier, Object parent)
            : base(identifier, parent)
        {
            ObjectRemoved = true;
        }
    }
}
