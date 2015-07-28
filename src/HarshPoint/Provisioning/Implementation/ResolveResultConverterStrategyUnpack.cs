using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyUnpack : ResolveResultConverterStrategy
    {
        private ResolveResultConverterStrategyUnpack() { }

        public override Object ConvertObject(Object obj)
            => NestedResolveResult.Unpack(obj);

        /// <remarks>This method is used by <see cref="ResolveResultConverterStrategyGrouping"/>
        /// to fetch simple (non-tuple) types.</remarks>
        public override Object ConvertNestedComponents(IEnumerator<Object> componentEnumerator)
            => componentEnumerator.GetNextOrFail();

        public static ResolveResultConverterStrategy Instance { get; }
            = new ResolveResultConverterStrategyUnpack();
    }
}
