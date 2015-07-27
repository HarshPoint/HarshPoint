using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyGrouping : ResolveResultConverterStrategy
    {
        private readonly Func<Object, IEnumerable<Object>, Object> _groupingFactory;
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

            _groupingFactory = CreateGroupingFactory(keyType, elementType);

            _keyStrategy = GetStrategyForType(keyType);
            _elementStrategy = GetStrategyForType(elementType);
        }

        public override IEnumerable<Object> ConvertResults(IEnumerable<Object> results)
            => base.ConvertResults(results)
                .Cast<Tuple<Object, ImmutableArray<Object>>>()
                .GroupBy(
                    tuple => tuple.Item1,
                    tuple => tuple.Item2,
                    (key, elements) => _groupingFactory(
                        key,
                        ConvertElements(elements).ToImmutableArray()
                    )
                )
                .ToImmutableArray();

        public override Object ConvertNestedComponents(IEnumerator<Object> componentEnumerator)
        {
            var key = _keyStrategy.ConvertNestedComponents(componentEnumerator);
            var value = componentEnumerator.TakeRemainder().ToImmutableArray();

            return Tuple.Create(key, value);
        }

        private IEnumerable<Object> ConvertElements(IEnumerable<ImmutableArray<Object>> elements)
            => _elementStrategy.ConvertResults(
                elements.Select(e => (Object)NestedResolveResult.FromComponents(e))
            );

        private static Func<Object, IEnumerable<Object>, Object> CreateGroupingFactory(Type keyType, Type elementType)
        {
            var keyParam = Expression.Parameter(typeof(Object));
            var elementsParam = Expression.Parameter(typeof(IEnumerable<Object>));

            var lambda = Expression.Lambda<Func<Object, IEnumerable<Object>, Object>>(
                Expression.Call(
                    null,
                    CreateGroupingMethodDefinition.MakeGenericMethod(keyType, elementType),
                    Expression.Convert(keyParam, keyType),
                    Expression.Call(
                        null,
                        CastMethodDefinition.MakeGenericMethod(elementType),
                        elementsParam
                    )
                ),
                keyParam,
                elementsParam
            );

            return lambda.Compile();
        }

        private static readonly MethodInfo CastMethodDefinition
            = typeof(Enumerable)
                .GetTypeInfo()
                .GetDeclaredMethods("Cast")
                .First(m =>
                {
                    var parameters = m.GetParameters();

                    return parameters.Length == 1 &&
                        parameters[0].ParameterType == typeof(IEnumerable);
                });

        private static readonly MethodInfo CreateGroupingMethodDefinition
            = typeof(HarshGrouping)
                .GetTypeInfo()
                .GetDeclaredMethods("Create")
                .First(m =>
                {
                    var lastParam = m.GetParameters().Last();
                    var lastParamType = lastParam.ParameterType;

                    return lastParamType.IsConstructedGenericType &&
                        lastParamType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                });

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategyGrouping>();
    }
}