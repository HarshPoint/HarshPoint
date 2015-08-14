using System;

namespace HarshPoint.Provisioning.Output
{
    public sealed class ObjectDidNotExist : IdentifiedOutputBase
    {
        public ObjectDidNotExist(String identifier, Object parent)
            : base(identifier, parent)
        {
        }
    }
}
