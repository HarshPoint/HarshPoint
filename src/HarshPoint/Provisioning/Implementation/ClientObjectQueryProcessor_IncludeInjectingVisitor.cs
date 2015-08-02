using HarshPoint.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class IncludeInjectingVisitor : ExpressionVisitor
        {
            private IncludeInjectingVisitor() { }

            protected override Expression VisitConstant(ConstantExpression node)
                => WrapWithIncludeCall(node);

            protected override Expression VisitMember(MemberExpression node)
                => WrapWithIncludeCall(node);

            protected override Expression VisitMethodCall(MethodCallExpression node)
                => WrapWithIncludeCall(node);

            private Expression WrapWithIncludeCall(Expression expression)
            {
                var elementType = HarshQueryable
                    .ExtractElementTypes(expression.Type)
                    .FirstOrDefault();

                if (elementType == null)
                {
                    return expression;
                }

                var includeCall = IncludeMethodCallExpression.TryExtend(expression);

                if (includeCall == null)
                {
                    includeCall = new IncludeMethodCallExpression(elementType, expression);
                }

                includeCall = includeCall.Update(
                    includeCall.Object,
                    Visit(includeCall.Retrievals)
                );

                return includeCall.Reduce();
            }

            public static IncludeInjectingVisitor Instance { get; }
                = new IncludeInjectingVisitor();
        }
    }
}