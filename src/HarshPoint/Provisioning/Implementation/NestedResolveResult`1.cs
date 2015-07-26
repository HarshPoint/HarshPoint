using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class NestedResolveResult<T> : INestedResolveResult
    {
        public NestedResolveResult(T value, IImmutableList<Object> parents)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            if (parents == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parents));
            }

            Value = value;
            Parents = parents;
        }

        public T Value { get; private set; }

        public IImmutableList<Object> Parents { get; private set; }

        Object INestedResolveResult.Value => Value;

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(NestedResolveResult<>));
    }
}
