using System;

namespace HarshPoint.Provisioning.Records
{
    public abstract class PropertyRecord<T> : HarshProvisionerRecord
    {
        protected PropertyRecord(T @object)
        {
            if (@object == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(@object));
            }

            Object = @object;
        }

        public T Object { get; }

        public sealed override Type ObjectType => Object?.GetType() ?? typeof(T);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyRecord<>));
    }
}
