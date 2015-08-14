using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectAdded<T> : IdentifiedObjectOutputBase<T>
    {
        public ObjectAdded(String identifier, Object parent, T @object)
            : base(identifier, parent, @object)
        {
            ObjectAdded = true;
        }
    }
}
