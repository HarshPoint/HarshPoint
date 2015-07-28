using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyTuple : ResolveResultConverterStrategy
    {
        private readonly ImmutableArray<ResolveResultConverterStrategy> _componentStrategies;

        public ResolveResultConverterStrategyTuple(Type tupleType)
            : base(tupleType)
        {
            _componentStrategies = HarshTuple.GetComponentTypes(tupleType)
                .Select(GetNestedTupleStrategy)
                .ToImmutableArray();
        }

        public override Object ConvertNestedComponents(IEnumerator<Object> componentEnumerator)
            => HarshTuple.Create(
                ResultType,
                _componentStrategies.Select(
                    s => s.ConvertNestedComponents(componentEnumerator)
                )
            );

        private ResolveResultConverterStrategy GetNestedTupleStrategy(Type t)
        {
            if (HarshGrouping.IsGroupingType(t))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(t),
                    SR.ResolveResultConverterStrategyTuple_NestedGroupingNotAllowed,
                    ResultType
                );
            }

            if (HarshTuple.IsTupleType(t))
            {
                return new ResolveResultConverterStrategyTuple(t);
            }

            return ResolveResultConverterStrategyUnpack.Instance;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategyTuple>();
    }
}
