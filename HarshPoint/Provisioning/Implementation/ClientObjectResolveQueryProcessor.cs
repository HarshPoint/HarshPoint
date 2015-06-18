using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectResolveQueryProcessor
    {
        private readonly ClientObjectResolveContext _context;

        private Expression _currentExpression;

        public ClientObjectResolveQueryProcessor(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            _context = context;
        }

        public Expression AddContextRetrievals(Expression expression)
        {
            if (expression == null)
            {
                throw Error.ArgumentNull(nameof(expression));
            }

            try
            {
                _currentExpression = expression;

                var visitor = new Visitor(this);
                var result = visitor.Visit(expression);

                var missingIncludes = _context
                    .GetRetrievalTypes()
                    .Except(visitor.IncludeCallsFound);

                if (missingIncludes.Any())
                {
                    throw Error.ArgumentOutOfRangeFormat(
                        nameof(expression),
                        SR.ClientObjectResolveQueryProcessor_MissingIncludeCalls,
                        String.Join("\n", missingIncludes.Select(t => t.Name)),
                        expression
                    );
                }

                return result;
            }
            finally
            {
                _currentExpression = null;
            }
        }

        private sealed class Visitor : ExpressionVisitor
        {
            public Visitor(ClientObjectResolveQueryProcessor owner)
            {
                Owner = owner;
                IncludeCallsFound = new HashSet<Type>();
            }

            public ClientObjectResolveQueryProcessor Owner
            {
                get;
                private set;
            }

            public HashSet<Type> IncludeCallsFound
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

                var retrievedType = node.Method.GetGenericArguments().Single();

                if (IncludeCallsFound.Contains(retrievedType))
                {
                    throw Error.ArgumentOutOfRangeFormat(
                        nameof(node),
                        SR.ClientObjectResolveQueryProcessor_MoreIncludeCallsNotSupported,
                        retrievedType,
                        Owner._currentExpression
                    );
                }

                var retrievals = node.Arguments[1] as NewArrayExpression;

                if (retrievals == null)
                {
                    throw Error.ArgumentOutOfRangeFormat(
                       nameof(node),
                       SR.ClientObjectResolveQueryProcessor_IncludeArgNotArray,
                       node,
                       Owner._currentExpression
                   );
                }

                IncludeCallsFound.Add(retrievedType);

                var retrievalsCombined = new ReadOnlyCollection<Expression>(
                    retrievals.Expressions
                    .Concat(Owner._context.GetRetrievals(retrievedType))
                    .ToArray()
                );

                if (!retrievalsCombined.Any())
                {
                    return Visit(node.Arguments[0]);
                }

                return Expression.Call(
                    null,
                    node.Method,
                    Visit(node.Arguments[0]),
                    Expression.NewArrayInit(
                        typeof(Expression<>).MakeGenericType(
                            typeof(Func<,>).MakeGenericType(
                                retrievedType,
                                typeof(Object)
                            )
                        ),
                        Visit(retrievalsCombined)
                    )
                );
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

                if (node.Arguments.Count != 2)
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

                return true;
            }
        }
    }
}