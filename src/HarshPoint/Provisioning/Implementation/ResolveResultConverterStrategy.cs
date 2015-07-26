using System;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultConverterStrategy
    {
        public abstract Object Convert(Object obj);

        public static ResolveResultConverterStrategy ForType(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (HarshTuple.IsTupleType(type))
            {
                throw new NotImplementedException();
            }

            return Unpack;
        }

        private sealed class UnpackStrategy : ResolveResultConverterStrategy
        {
            public override Object Convert(Object obj)
                => NestedResolveResult.Unpack(obj);
        }

        private static readonly ResolveResultConverterStrategy Unpack = new UnpackStrategy();

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategy>();
    }
}
