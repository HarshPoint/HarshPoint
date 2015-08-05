using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectCreated<T> : IdentifiedObjectOutputBase<T>
    {
        public ObjectCreated(String identifier, T @object)
            : base(identifier, @object)
        {
        }
    }
}
