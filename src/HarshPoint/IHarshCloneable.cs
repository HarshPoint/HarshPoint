using System;
using System.Linq.Expressions;

namespace HarshPoint
{
    public interface IHarshCloneable
    {
        Object Clone();
    }

    public static class HarshCloneable
    {
        public static T With<T>(this T cloneable, Action<T> modifier)
            where T : IHarshCloneable
        {
            if (cloneable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(cloneable));
            }

            if (modifier == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(modifier));
            }

            var clone = (T)cloneable.Clone();
            modifier(clone);
            return clone;
        }

        public static T With<T, TValue>(this T cloneable, Expression<Func<T, TValue>> expression, TValue value)
            where T : IHarshCloneable
        {
            if (cloneable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(cloneable));
            }

            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var oldValue = expression.Compile()(cloneable);

            if (Equals(oldValue, value))
            {
                return cloneable;
            }

            var field = expression.TryExtractSingleFieldAccess();
            if (field != null)
            {
                return With(
                    cloneable,
                    clone => field.SetValue(clone, value)
                );
            }

            var property = expression.TryExtractSinglePropertyAccess();
            if (property != null)
            {
                return With(
                    cloneable,
                    clone => property.SetValue(clone, value)
                );
            }

            throw Logger.Fatal.ArgumentFormat(
                nameof(expression),
                SR.HarshCloneable_ExpressionNotFieldOrProperty,
                expression
            );
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshCloneable));
    }
}
