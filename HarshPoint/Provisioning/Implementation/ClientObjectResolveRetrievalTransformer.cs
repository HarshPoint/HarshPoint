using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectResolveRetrievalTransformer<T> : ExpressionVisitor
    {
        private readonly Expression<Func<T, Object>>[] _retrievals;

        public ClientObjectResolveRetrievalTransformer(params Expression<Func<T, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Error.ArgumentNull(nameof(retrievals));
            }

            _retrievals = retrievals;
        }

        public Expression Process(Expression expression)
        {
            if (expression == null)
            {
                throw Error.ArgumentNull(nameof(expression));
            }

            var visitor = new Visitor(_retrievals);
            var result = visitor.Visit(expression);

            if (!visitor.IncludeCallFound)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    nameof(expression),
                    SR.ClientObjectResolveQueryRetrievalVisitor_NoIncludeCall,
                    expression
                );
            }

            return result;
        }

        private sealed class Visitor : ExpressionVisitor
        {
            private readonly Expression<Func<T, Object>>[] _retrievals;

            public Visitor(Expression<Func<T, Object>>[] retrievals)
            {
                _retrievals = retrievals;
            }

            public MethodCallExpression IncludeCall
            {
                get;
                private set;
            }

            public Boolean IncludeCallFound
            {
                get;
                private set;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw Error.ArgumentNull(nameof(node));
                }

                if (!IsIncludeOrIncludeWithDefaultProperties(node))
                {
                    return base.VisitMethodCall(node);
                }

                if (IncludeCallFound)
                {
                    throw Error.ArgumentOutOfRangeFormat(
                        nameof(node),
                        SR.ClientObjectResolveQueryRetrievalVisitor_MoreIncludeCallsNotSupported,
                        IncludeCall,
                        node
                    );
                }

                var retrievals = node.Arguments[1] as NewArrayExpression;

                if (retrievals == null)
                {
                    throw Error.ArgumentOutOfRangeFormat(
                       nameof(node),
                       SR.ClientObjectResolveQueryRetrievalVisitor_IncludeArgNotArray,
                       node
                   );
                }

                IncludeCallFound = true;

                var retrievalsCombined = new ReadOnlyCollection<Expression>(
                    retrievals.Expressions
                    .Concat(_retrievals)
                    .Distinct()
                    .ToArray()
                );

                if (!retrievalsCombined.Any())
                {
                    return Visit(node.Arguments[0]);
                }

                IncludeCall = Expression.Call(
                    null,
                    node.Method,
                    Visit(node.Arguments[0]),
                    Expression.NewArrayInit(
                        typeof(Expression<Func<T, Object>>),
                        Visit(retrievalsCombined)
                    )
                );

                return IncludeCall;
            }

            private static Boolean IsIncludeOrIncludeWithDefaultProperties(MethodCallExpression node)
            {
                if (node == null)
                {
                    return false;
                }

                if (!node.Method.DeclaringType.Equals(typeof(ClientObjectQueryableExtension)))
                {
                    return false;
                }

                if (!node.Method.Name.Equals("Include") &&
                    !node.Method.Name.Equals("IncludeWithDefaultProperties"))
                {
                    return false;
                }

                if (node.Arguments.Count < 2)
                {
                    return false;
                }

                if (!node.Method.IsGenericMethod)
                {
                    return false;
                }

                var genericArguments = node.Method.GetGenericArguments();

                if (genericArguments.Length != 1)
                {
                    return false;
                }

                if (genericArguments[0] != typeof(T))
                {
                    return false;
                }

                return true;
            }
        }
    }
}