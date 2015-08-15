using System;

namespace HarshPoint.Provisioning.Output
{
    public abstract class IdentifiedOutputBase : HarshProvisionerOutput
    {
        protected IdentifiedOutputBase(String identifier, Object parent)
        {
            if (String.IsNullOrWhiteSpace(identifier))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(identifier));
            }

            Identifier = identifier;
            Parent = parent;
        }

        public String Identifier { get; }
        public Boolean ObjectAdded { get; protected set; }
        public Boolean ObjectRemoved { get; protected set; }
        public Object Parent { get; }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(IdentifiedOutputBase));
    }
}
