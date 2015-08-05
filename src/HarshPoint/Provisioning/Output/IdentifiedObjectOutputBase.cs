using System;

namespace HarshPoint.Provisioning.Output
{
    public abstract class IdentifiedObjectOutputBase<T> : HarshProvisionerOutput
    {
        protected IdentifiedObjectOutputBase(String identifier, T @object)
        {
            if (String.IsNullOrWhiteSpace(identifier))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(identifier));
            }

            if (@object == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(@object));
            }

            Identifier = identifier;
            Object = @object;
        }

        public String Identifier { get; private set; }

        public T Object { get; private set; }

        public Boolean ObjectCreated { get; protected set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ObjectCreated<>));
    }
}
