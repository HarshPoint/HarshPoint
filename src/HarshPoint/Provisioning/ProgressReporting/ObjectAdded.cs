using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ObjectAdded<T> : IdentifiedObjectProgressReportBase<T>
    {
        public ObjectAdded(String identifier, Object parent, T @object)
            : base(identifier, parent, @object)
        {
            ObjectAdded = true;
        }
    }
}
