using HarshPoint.Linq;
using System;
using System.Linq.Expressions;
using Xunit;

namespace HarshPoint.Tests.Linq
{
    public class ExpressionComparing
    {
        [Fact]
        public void Equal_exprs_are_equal()
        {
            AssertEqual<String, Int32>(true, x => x.Length, x => x.Length);
            AssertEqual<String, Int32>(true, x => x.Normalize().Length, x => x.Normalize().Length);

            AssertEqual<String, Boolean>(false, x => x.Normalize().Length < 42, x => x.Normalize().Length > 42);
        }

        private void AssertEqual<T, TResult>(Boolean isEqual, Expression<Func<T, TResult>> x, Expression<Func<T, TResult>> y)
        {
            Assert.NotSame(x, y);
            Assert.Equal(isEqual, Comparer.Equals(x, y));
        }

        private static readonly HarshExpressionEqualityComparer Comparer
            = HarshExpressionEqualityComparer.Instance;
    }
}
