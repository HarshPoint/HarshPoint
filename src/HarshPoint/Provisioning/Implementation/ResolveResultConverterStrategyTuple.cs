using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyTuple : ResolveResultConverterStrategy
    {
        private readonly ImmutableArray<Func<IEnumerator<Object>, Object>> _nested;

        public ResolveResultConverterStrategyTuple(Type tupleType)
            : base(tupleType)
        {
            _nested = HarshTuple.GetComponentTypes(tupleType)
                .Select(GetNestedTupleStrategy)
                .ToImmutableArray();
        }

        protected override Object ConvertNestedComponents(IEnumerator<Object> componentEnumerator)
            => HarshTuple.Create(
                ResultType,
                _nested.Select(n => n(componentEnumerator))
            );

        private static Func<IEnumerator<Object>, Object> GetNestedTupleStrategy(Type t)
        {
            if (HarshTuple.IsTupleType(t))
            {
                return new ResolveResultConverterStrategyTuple(t).ConvertNestedComponents;
            }

            return enumerator => enumerator.MoveNext() ? enumerator.Current : null;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategyTuple>();
    }
}
