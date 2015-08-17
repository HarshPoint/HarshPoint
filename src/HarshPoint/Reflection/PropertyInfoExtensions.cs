using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static Func<Object, Object> MakeGetter(this PropertyInfo property)
            => MakeGetter<Object, Object>(property);

        public static LambdaExpression MakeGetterExpression(this PropertyInfo property)
            => MakeGetterExpression(property, null, null);

        public static LambdaExpression MakeGetterExpression(
            this PropertyInfo property,
            Type targetType = null,
            Type resultType = null
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (targetType == null)
            {
                targetType = property.DeclaringType;
            }

            if (resultType == null)
            {
                resultType = property.PropertyType;
            }

            var target = Expression.Parameter(targetType, "target");

            var memberAccess = Expression.MakeMemberAccess(
                ConvertIfNeeded(target, property.DeclaringType),
                property
            );

            var delegateType = typeof(Func<,>).MakeGenericType(
                targetType, resultType
            );

            var lambda = Expression.Lambda(
                delegateType,
                ConvertIfNeeded(memberAccess, resultType),
                target
            );

            return lambda;
        }

        public static Func<TTarget, TResult> MakeGetter<TTarget, TResult>(this PropertyInfo property)
            => MakeGetterExpression<TTarget, TResult>(property).Compile();

        public static Expression<Func<TTarget, TResult>> MakeGetterExpression<TTarget, TResult>(this PropertyInfo property)
            => (Expression<Func<TTarget, TResult>>)MakeGetterExpression(
                property,
                typeof(TTarget),
                typeof(TResult)
            );

        public static Action<Object, Object> MakeSetter(this PropertyInfo property)
            => MakeSetter<Object, Object>(property);

        public static Action<TTarget, TProperty> MakeSetter<TTarget, TProperty>(this PropertyInfo property)
            => MakeSetterExpression<TTarget, TProperty>(property).Compile();

        public static Expression<Action<Object, Object>> MakeSetterExpression(this PropertyInfo property)
            => MakeSetterExpression<Object, Object>(property);

        public static Expression<Action<TTarget, TProperty>> MakeSetterExpression<TTarget, TProperty>(this PropertyInfo property)
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            var target = Expression.Parameter(typeof(TTarget), "target");
            var value = Expression.Parameter(typeof(TProperty), "object");

            var lambda = Expression.Lambda<Action<TTarget, TProperty>>(
                Expression.Assign(
                    Expression.MakeMemberAccess(
                        ConvertIfNeeded(target, property.DeclaringType),
                        property
                    ),
                    ConvertIfNeeded(value, property.PropertyType)
                ),
                target,
                value
            );

            return lambda;
        }

        private static Expression ConvertIfNeeded(Expression expression, Type type)
        {
            if (expression.Type == type)
            {
                return expression;
            }

            return Expression.ConvertChecked(expression, type);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(PropertyInfoExtensions));
    }
}
