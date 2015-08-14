using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectAlreadyExists<T> : IdentifiedObjectOutputBase<T>
    {
        public ObjectAlreadyExists(String identifier, Object parent, T @object)
            : base(identifier, parent, @object)
        {
        }
    }
}
