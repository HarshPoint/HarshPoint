using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectAlreadyExists<T> : IdentifiedObjectOutputBase<T>
    {
        public ObjectAlreadyExists(String identifier, T @object) 
            : base(identifier, @object)
        {
        }
    }
}
