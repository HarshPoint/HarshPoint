using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveFailure
    {
        public ResolveFailure(Object resolvable)
            : this(resolvable, null)
        {
        }

        public ResolveFailure(Object resolvable, Object identifier)
        {
            if (resolvable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolvable));
            }

            Resolvable = resolvable;
            Identifier = identifier;
        }

        public override String ToString()
        {
            if (Identifier != null)
            {
                return String.Concat(Resolvable, ": ", Identifier);
            }

            return Resolvable.ToString();
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

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveFailure>();
    }
}
