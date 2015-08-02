using HarshPoint.Diagnostics;
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
            public RetrievalAppendingVisitor(ClientObjectQueryProcessor owner, DepthLimiter depthLimiter = null)
            {
                Owner = owner;
                DepthLimiter = depthLimiter ?? owner.CreateDepthLimiter();
            }

            public DepthLimiter DepthLimiter { get; private set; }

            public ClientObjectQueryProcessor Owner { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(node));
                }

                return Logger.MethodCall(nameof(VisitMethodCall), node).Call(() =>
                {
                    var callInfo = new MethodCallInfo(node);

                    if (callInfo.IsInclude)
                    {
                        return VisitIncludeCall(node, callInfo);
                    }

                    return base.VisitMethodCall(node);
                });

            }

            private Expression VisitIncludeCall(MethodCallExpression node, MethodCallInfo callInfo)
            {
                var retrievals = GetExistingRetrievals(node);
                var canRecurse = DepthLimiter.CanRecurse(callInfo.ElementType);

                using (DepthLimiter.Enter(callInfo.ElementType))
                {
                    if (canRecurse)
                    {
                        // TODO: remove duplicates
                        retrievals = new ReadOnlyCollection<Expression>(
                            retrievals
                            .Concat(
                                Owner.GetRetrievals(callInfo.ElementType, DepthLimiter)
                            )
                            .ToArray()
                        );
                    }

                    Logger.Debug(
                        "RetrievalAppendingVisitor retrievals to include: {Retrievals}",
                        retrievals
                    );

                    if (retrievals.Any())
                    {
                        return CreateIncludeCall(node, callInfo, retrievals);
                    }

                    return Visit(node.Arguments[0]);
                }
            }

            private Expression CreateIncludeCall(MethodCallExpression node, MethodCallInfo callInfo, ReadOnlyCollection<Expression> retrievals)
                => node.Update(null, new[]
                    {
                        Visit(node.Arguments[0]),
                        Expression.NewArrayInit(
                            typeof(Expression<>).MakeGenericType(
                                typeof(Func<,>).MakeGenericType(
                                    callInfo.ElementType,
                                    typeof(Object)
                                )
                            ),
                            Visit(retrievals)
                        )
                    }
                );

            private ReadOnlyCollection<Expression> GetExistingRetrievals(MethodCallExpression node)
            {
                var newArray = node.Arguments[1] as NewArrayExpression;

                if (newArray == null)
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

                return newArray.Expressions;
            }
        }
    }
}