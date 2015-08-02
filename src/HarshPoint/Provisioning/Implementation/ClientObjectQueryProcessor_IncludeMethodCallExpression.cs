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
        private sealed class IncludeMethodCallExpression : Expression
        {
            private IncludeMethodCallExpression(MethodCallExpression node)
            {
                Method = node.Method;
                Object = node.Arguments[0];
                Retrievals = ExtractRetrievals(node);
            }

            private IncludeMethodCallExpression(MethodInfo method, Expression @object, ReadOnlyCollection<Expression> retrievals)
            {
                Method = method;
                Object = @object;
                Retrievals = retrievals;
            }

            public IncludeMethodCallExpression(Type elementType, Expression @object, params Expression[] retrievals)
            {
                if (elementType == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(elementType));
                }

                if (@object == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(@object));
                }

                if (retrievals == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(retrievals));
                }

                Method = IncludeMethodDefinition.MakeGenericMethod(elementType);
                Object = @object;
                Retrievals = new ReadOnlyCollection<Expression>(retrievals);
            }

            public override Boolean CanReduce => true;

            public Type ElementType => Method.GetGenericArguments()[0];

            public MethodInfo Method { get; private set; }

            public override ExpressionType NodeType => ExpressionType.Extension;

            public Expression Object { get; private set; }

            public override Type Type => Method.ReturnType;

            public ReadOnlyCollection<Expression> Retrievals { get; private set; }

            public override Expression Reduce()
                => Expression.Call(
                    null,
                    Method,
                    Object,
                    RetrievalsToNewArray()
                );

            public IncludeMethodCallExpression Update(Expression @object, ReadOnlyCollection<Expression> retrievals)
            {
                if (@object == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(@object));
                }

                if (retrievals == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(retrievals));
                }

                return new IncludeMethodCallExpression(
                    Method,
                    @object,
                    retrievals
                );
            }

            private NewArrayExpression RetrievalsToNewArray()
                => Expression.NewArrayInit(
                    typeof(Expression<>).MakeGenericType(
                        typeof(Func<,>).MakeGenericType(
                            ElementType,
                            typeof(Object)
                        )
                    ),
                    Retrievals
                );

            public static IncludeMethodCallExpression TryExtend(Expression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(node));
                }

                var methodCall = node as MethodCallExpression;

                if (methodCall != null)
                {
                    return TryExtend(methodCall);
                }

                return null;
            }

            public static IncludeMethodCallExpression TryExtend(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(node));
                }

                if (!node.Method.DeclaringType.Equals(typeof(ClientObjectQueryableExtension)))
                {
                    return null;
                }

                if (node.Arguments.Count != 2)
                {
                    return null;
                }

                if (!node.Method.IsGenericMethod)
                {
                    return null;
                }

                var genericArguments = node.Method.GetGenericArguments();

                if (genericArguments.Length != 1)
                {
                    return null;
                }

                var isInclude = 
                    node.Method.Name.Equals("IncludeWithDefaultProperties") ||
                    node.Method.Name.Equals("Include");

                if (!isInclude)
                {
                    return null;
                }

                return new IncludeMethodCallExpression(node);
            }

            private static ReadOnlyCollection<Expression> ExtractRetrievals(MethodCallExpression node)
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

                return newArray.Expressions;
            }

            private static readonly MethodInfo IncludeMethodDefinition 
                = typeof(ClientObjectQueryableExtension)
                    .GetTypeInfo()
                    .GetDeclaredMethods("Include")
                    .FirstOrDefault(m =>
                        m.IsStatic &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2
                    );

            private static readonly HarshLogger Logger = HarshLog.ForContext<IncludeMethodCallExpression>();
        }
    }
}