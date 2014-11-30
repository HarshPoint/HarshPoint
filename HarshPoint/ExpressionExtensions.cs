using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace HarshPoint
{
    public static class ExpressionExtensions
    {
        public static String GetMemberName<T, TResult>(this Expression<Func<T, TResult>> expression)
        {
            return GetMemberNameCore(expression);
        }

        public static String GetMemberName<T>(this Expression<Func<T>> expression)
        {
            return GetMemberNameCore(expression);
        }

        private static String GetMemberNameCore(Expression expression)
        {
            if (expression == null)
            {
                throw Error.ArgumentNull("expression");
            }

            var visitor = new MemberNameVisitor();
            visitor.Visit(expression);

            if (visitor.MemberNames == null)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "expression",
                    expression,
                    SR.ExpressionExtensions_MemberExpressionNotFound
                );
            }

            return String.Join(".", visitor.MemberNames);
        }

        private sealed class MemberNameVisitor : ExpressionVisitor
        {
            public MemberNameVisitor()
            {
                MemberNames = ImmutableStack.Create<String>();
            }

            public ImmutableStack<String> MemberNames
            {
                get;
                private set;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member != null)
                {
                    MemberNames = MemberNames.Push(node.Member.Name);
                }

                return base.VisitMember(node);
            }
        }
    }
}
