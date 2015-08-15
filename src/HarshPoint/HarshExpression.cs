using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint
{
    public static class HarshExpression
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static PropertyInfo TryExtractSinglePropertyAccess(Expression<Func<Object>> expression)
            => expression.TryExtractSinglePropertyAccess();
    }
}
