using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveFailure
    {
        public ResolveFailure(IResolveBuilder resolveBuilder)
            : this(resolveBuilder, null)
        {
        }

        public ResolveFailure(IResolveBuilder resolveBuilder, Object identifier)
        {
            if (resolveBuilder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveBuilder));
            }

            ResolveBuilder = resolveBuilder;
            Identifier = identifier;
        }

        public override String ToString()
        {
            if (Identifier != null)
            {
                return String.Concat(ResolveBuilder, ": ", Identifier);
            }

            return ResolveBuilder.ToString();
        }

        public Object Identifier
        {
            get;

        }

        public Object ResolveBuilder
        {
            get;

        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveFailure>();
    }
}
