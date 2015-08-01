using HarshPoint.Linq;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

                var instance = expression;
                var retrievals = new ReadOnlyCollection<Expression>(new Expression[0]);

                var methodCall = expression as MethodCallExpression;
                var includeCall = new MethodCallInfo(methodCall);

                if (includeCall.IsInclude)
                {
                    var arrayInit = methodCall.Arguments[1] as NewArrayExpression;

                    if (arrayInit == null)
                    {
                        throw Logger.Fatal.ArgumentFormat(
                            nameof(expression),
                            SR.ClientObjectResolveQueryProcessor_IncludeArgNotArray,
                            methodCall
                        );
                    }

                    instance = methodCall.Arguments[0];
                    retrievals = Visit(arrayInit.Expressions);
                }


                return Expression.Call(
                    null,
                    IncludeMethod.MakeGenericMethod(elementType),
                    instance,
                    Expression.NewArrayInit(
                        typeof(Expression<>).MakeGenericType(
                            typeof(Func<,>).MakeGenericType(
                                elementType,
                                typeof(Object)
                            )
                        ),
                        retrievals
                    )
                );
            }

            public static IncludeInjectingVisitor Instance { get; } = new IncludeInjectingVisitor();

            private static readonly MethodInfo IncludeMethod =
                typeof(ClientObjectQueryableExtension)
                .GetTypeInfo()
                .GetDeclaredMethods("Include")
                .FirstOrDefault(m =>
                    m.IsStatic &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2
                );
        }
    }
}