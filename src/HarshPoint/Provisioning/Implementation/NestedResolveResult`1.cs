using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class NestedResolveResult<T> : NestedResolveResult
    {
        public NestedResolveResult(T value, IImmutableList<Object> parents)
            : base(value, parents)
        {
        }

        public new T Value => (T)base.Value;

        public override Type ValueType => typeof(T);
    }
}
