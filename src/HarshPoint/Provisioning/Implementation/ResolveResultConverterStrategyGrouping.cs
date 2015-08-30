using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyGrouping : ResolveResultConverterStrategy
    {
        private readonly HarshGroupingDynamicFactory _groupingFactory;
        private readonly ResolveResultConverterStrategy _elementStrategy;
        private readonly ResolveResultConverterStrategy _keyStrategy;

        public ResolveResultConverterStrategyGrouping(Type resultType)
            : base(resultType)
        {
            if (resultType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resultType));
            }

            if (!HarshGrouping.IsGroupingType(resultType))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(resultType),
                    SR.ResolveResultConverterStrategyGrouping_NotAGrouping,
                    resultType
                );
            }

            var keyType = resultType.GenericTypeArguments[0];
            var elementType = resultType.GenericTypeArguments[1];

            _groupingFactory = new HarshGroupingDynamicFactory(keyType, elementType);

            _keyStrategy = GetStrategyForType(keyType);
            _elementStrategy = GetStrategyForType(elementType);
        }

        public override IEnumerable<Object> ConvertResults(IEnumerable<Object> results)
            => base.ConvertResults(results)
                .Cast<Tuple<Object, ImmutableArray<Object>>>()
                .GroupBy(
                    tuple => tuple.Item1,
                    tuple => tuple.Item2,
                    (key, elements) => _groupingFactory.Create(
                        key,
                        ConvertElements(elements).ToImmutableArray()
                    )
                )
                .ToImmutableArray();

        public override Object ConvertNestedComponents(
            NestedResolveResult nested,
            IEnumerator<Object> componentEnumerator
        )
        {
            var key = _keyStrategy.ConvertNestedComponents(
                nested,
                componentEnumerator
            );

            var value = componentEnumerator.TakeRemainder().ToImmutableArray();

            return Tuple.Create(key, value);
        }

        private IEnumerable<Object> ConvertElements(IEnumerable<ImmutableArray<Object>> elements)
            => _elementStrategy.ConvertResults(
                elements.Select(e => (Object)NestedResolveResult.FromComponents(e))
            );
        
        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategyGrouping>();
    }
}