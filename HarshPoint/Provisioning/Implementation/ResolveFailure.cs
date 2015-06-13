using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveFailure
    {
        public ResolveFailure(Object resolvable, Object identifier)
        {
            if (resolvable == null)
            {
                throw Error.ArgumentNull(nameof(resolvable));
            }

            if (identifier == null)
            {
                throw Error.ArgumentNull(nameof(identifier));
            }

            Resolvable = resolvable;
            Identifier = identifier;
        }

        public Object Identifier
        {
            get;
            private set;
        }
        public Object Resolvable
        {
            get;
            private set;
        }
    }
}
