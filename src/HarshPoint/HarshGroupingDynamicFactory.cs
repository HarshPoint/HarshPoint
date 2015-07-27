using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint
{
    public sealed class HarshGroupingDynamicFactory
    {
        private delegate Object FactoryFunc(Object key, IEnumerable elments);

        private readonly Type _keyType;
        private readonly Type _elementType;
        private readonly Lazy<FactoryFunc> _factory;

        public HarshGroupingDynamicFactory(Type keyType, Type elementType)
        {
            if (keyType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(keyType));
            }

            if (elementType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(elementType));
            }

            _keyType = keyType;
            _elementType = elementType;
            _factory = new Lazy<FactoryFunc>(CreateFactory);
        }

        public Object Create(Object key, IEnumerable elements)
        {
            if(elements == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(elements));
            }

            return _factory.Value(key, elements);
        }

        private FactoryFunc CreateFactory()
        {
            var keyParam = Expression.Parameter(typeof(Object));
            var elementsParam = Expression.Parameter(typeof(IEnumerable));

            var lambda = Expression.Lambda<FactoryFunc>(
                Expression.Call(
                    null,
                    CreateGroupingMethodDefinition.MakeGenericMethod(_keyType, _elementType),
                    Expression.Convert(keyParam, _keyType),
                    Expression.Call(
                        null,
                        CastMethodDefinition.MakeGenericMethod(_elementType),
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

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshGroupingDynamicFactory>();
    }
}
