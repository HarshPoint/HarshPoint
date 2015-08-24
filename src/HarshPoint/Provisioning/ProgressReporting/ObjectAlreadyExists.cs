using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public sealed class ObjectAlreadyExists<T> : IdentifiedObjectProgressReportBase<T>
    {
        public ObjectAlreadyExists(String identifier, Object parent, T @object)
            : base(identifier, parent, @object)
        {
        }
    }
}
