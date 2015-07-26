using System;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyUnpack : ResolveResultConverterStrategy
    {
        private ResolveResultConverterStrategyUnpack() { }

        protected override Object ConvertObject(Object obj)
            => NestedResolveResult.Unpack(obj);

        public static ResolveResultConverterStrategy Instance { get; }
            = new ResolveResultConverterStrategyUnpack();
    }
}
