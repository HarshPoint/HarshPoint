﻿using System;
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static String GetMemberName<T, TResult>(this Expression<Func<T, TResult>> expression)
        {
            return GetMemberName((Expression)expression);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static String GetMemberName<T>(this Expression<Func<T>> expression)
        {
            return GetMemberName((Expression)expression);
        }

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
                throw Error.ArgumentNull(nameof(expression));
            }

            var visitor = new MemberNameVisitor();
            visitor.Visit(expression);

            if (visitor.Members.IsEmpty)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    nameof(expression),
                    SR.ExpressionExtensions_MemberExpressionNotFound
                );
            }

            return visitor.Members;
        }

        private sealed class MemberNameVisitor : ExpressionVisitor
        {
            public MemberNameVisitor()
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
                    throw Error.ArgumentNull("node");
                }

                if (node.Member != null)
                {
                    Members = Members.Push(node.Member);
                }

                return base.VisitMember(node);
            }
        }
    }
}