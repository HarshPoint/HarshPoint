using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectRemoved : IdentifiedOutputBase
    {
        public ObjectRemoved(String identifier, Object parent) 
            : base(identifier, parent)
        {
            ObjectRemoved = true;
        }
    }
}
