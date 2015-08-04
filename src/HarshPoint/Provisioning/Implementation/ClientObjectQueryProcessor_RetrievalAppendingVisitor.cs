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
                DepthLimiter = new DepthLimiter(owner);
            }

            public DepthLimiter DepthLimiter { get; private set; }

            public ClientObjectQueryProcessor Owner { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(node));
                }

                return Logger.Method(nameof(VisitMethodCall), node).Invoke(() =>
                {
                    var includeCall = IncludeMethodCallExpression.TryExtend(node);

                    if (includeCall != null)
                    {
                        return VisitIncludeCall(includeCall);
                    }

                    return base.VisitMethodCall(node);
                });

            }

            private Expression VisitIncludeCall(IncludeMethodCallExpression includeCall)
            {
                var retrievals = includeCall.Retrievals;
                var canRecurse = DepthLimiter.CanRecurse(includeCall.ElementType);

                using (DepthLimiter.Enter(includeCall.ElementType))
                {
                    if (canRecurse)
                    {
                        // TODO: remove duplicates
                        retrievals = new ReadOnlyCollection<Expression>(
                            retrievals
                            .Concat(
                                Owner.GetRetrievalsCore(includeCall.ElementType)
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
                        var updated = includeCall.Update(
                            Visit(includeCall.Object), 
                            Visit(retrievals)
                        );

                        return updated.Reduce();
                    }

                    return Visit(includeCall.Object);
                }
            }
        }
    }
}