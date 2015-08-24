using System;

namespace HarshPoint.Provisioning.ProgressReporting
{
    public abstract class IdentifiedObjectProgressReportBase<T> : IdentifiedProgressReportBase
    {
        protected IdentifiedObjectProgressReportBase(String identifier, Object parent, T @object)
            : base(identifier, parent)
        {
            if (@object == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(@object));
            }

            Object = @object;
        }

        public T Object { get; set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(IdentifiedObjectProgressReportBase<>));
    }
}
