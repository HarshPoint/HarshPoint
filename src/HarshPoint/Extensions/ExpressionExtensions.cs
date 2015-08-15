using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint
{
    public static class ExpressionExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static Expression<Func<T, Object>> ConvertToObject<T, TResult>(
            this Expression<Func<T, TResult>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return Expression.Lambda<Func<T, Object>>(
                Expression.Convert(
                    expression.Body,
                    typeof(Object)
                ),
                expression.Parameters
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static Expression<Func<T, IEnumerable<TResult>>> ConvertToSingleElementArray<T, TResult>(
            this Expression<Func<T, TResult>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(
                Expression.NewArrayInit(
                    typeof(TResult),
                    expression.Body
                ),
                expression.Parameters
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static String GetMemberName<T, TResult>(this Expression<Func<T, TResult>> expression)
            => GetMemberName((Expression)expression);

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static String GetMemberName<T>(this Expression<Func<T>> expression)
            => GetMemberName((Expression)expression);

        public static String GetMemberName(this Expression expression)
        {
            var memberNames =
                ExtractMemberAccess(expression)
                .Select(m => m.Name);

            return String.Join(".", memberNames);
        }

        public static IEnumerable<MemberInfo> ExtractMemberAccess(this Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var visitor = new ExtractMemberAccessVisitor();
            visitor.Visit(expression);

            if (visitor.Members.IsEmpty)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(expression),
                    SR.ExpressionExtensions_MemberExpressionNotFound
                );
            }

            return visitor.Members;
        }

        public static PropertyInfo ExtractSinglePropertyAccess(this Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return (PropertyInfo)ExtractMemberAccess(expression).First();
        }

        public static FieldInfo TryExtractSingleFieldAccess(this Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return ExtractMemberAccess(expression).First() as FieldInfo;
        }

        public static PropertyInfo TryExtractSinglePropertyAccess(this Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return ExtractMemberAccess(expression).First() as PropertyInfo;
        }

        private sealed class ExtractMemberAccessVisitor : ExpressionVisitor
        {
            public ExtractMemberAccessVisitor()
            {
                Members = ImmutableStack.Create<MemberInfo>();
            }

            public ImmutableStack<MemberInfo> Members
            {
                get;
                private set;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull("node");
                }

                if (node.Member != null)
                {
                    Members = Members.Push(node.Member);
                }

                return base.VisitMember(node);
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ExpressionExtensions));
    }
}
