using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static Func<TTarget, TProperty> MakeGetter<TTarget, TProperty>(this PropertyInfo property)
        {
            if (property == null)
            {
                throw Error.ArgumentNull(nameof(property));
            }

            var target = Expression.Parameter(typeof(TTarget));

            var lambda = Expression.Lambda<Func<TTarget, TProperty>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(target, property.DeclaringType),
                        property.GetMethod
                    ),
                    typeof(TProperty)
                ),
                target
            );

            return lambda.Compile();
        }

        public static Func<Object, Object> MakeGetter(this PropertyInfo property)
            => MakeGetter<Object, Object>(property);
    
        public static Action<TTarget, TProperty> MakeSetter<TTarget, TProperty>(this PropertyInfo property)
        {
            if (property == null)
            {
                throw Error.ArgumentNull(nameof(property));
            }

            var target = Expression.Parameter(typeof(TTarget));
            var value = Expression.Parameter(typeof(TProperty));

            var lambda = Expression.Lambda<Action<TTarget, TProperty>>(
                Expression.Call(
                    Expression.Convert(target, property.DeclaringType),
                    property.SetMethod,
                    Expression.Convert(value, property.PropertyType)
                ),
                target,
                value
            );

            return lambda.Compile();
        }

        public static Action<Object, Object> MakeSetter(this PropertyInfo property)
            => MakeSetter<Object, Object>(property);
    }
}
