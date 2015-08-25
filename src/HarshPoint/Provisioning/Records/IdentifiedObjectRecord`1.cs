using System;

namespace HarshPoint.Provisioning.Records
{
    public abstract class IdentifiedObjectRecord<T> : IdentifiedRecord
    {
        protected IdentifiedObjectRecord(String identifier, T @object)
            : base(identifier)
        {
            if (@object == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(@object));
            }

            Object = @object;
        }

        public T Object { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(IdentifiedObjectRecord<>));
    }
}
