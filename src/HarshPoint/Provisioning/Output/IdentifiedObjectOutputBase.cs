using System;

namespace HarshPoint.Provisioning.Output
{
    public abstract class IdentifiedObjectOutputBase<T> : IdentifiedOutputBase
    {
        protected IdentifiedObjectOutputBase(String identifier, Object parent, T @object)
            : base(identifier, parent)
        {
            if (@object == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(@object));
            }

            Object = @object;
        }

        public T Object { get; private set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(IdentifiedObjectOutputBase<>));
    }
}
