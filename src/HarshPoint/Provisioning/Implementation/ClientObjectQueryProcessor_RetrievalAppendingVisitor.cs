using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class RetrievalAppendingVisitor : ExpressionVisitor
        {
            public RetrievalAppendingVisitor(ClientObjectQueryProcessor owner)
            {
                Owner = owner;
            }

            public ClientObjectQueryProcessor Owner { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(node));
                }

                var callInfo = new MethodCallInfo(node);

                if (!callInfo.IsInclude)
                {
                    return base.VisitMethodCall(node);
                }

                var retrievals = node.Arguments[1] as NewArrayExpression;

                if (retrievals == null)
                {
                    throw Logger.Fatal.ArgumentFormat(
                       nameof(node),
                       SR.ClientObjectResolveQueryProcessor_IncludeArgNotArray,
                       node
                   );
                }

                Logger.Debug(
                    "RetrievalAppendingVisitor processing {Expression}",
                    node
                );

                var retrievalsCombined = new ReadOnlyCollection<Expression>(
                    retrievals.Expressions
                    .Concat(
                        Owner.GetRetrievals(callInfo.ElementType)
                    )
                    .ToArray()
                );

                Logger.Debug(
                    "RetrievalAppendingVisitor retrievals to include: {Retrievals}",
                    retrievalsCombined
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
                                callInfo.ElementType,
                                typeof(Object)
                            )
                        ),
                        Visit(retrievalsCombined)
                    )
                );
            }
        }
    }
}