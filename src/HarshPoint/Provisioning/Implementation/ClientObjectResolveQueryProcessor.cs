using HarshPoint.Reflection;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ClientObjectResolveQueryProcessor
    {
        private IImmutableDictionary<Type, IImmutableList<Expression>> _retrievals;

        public ClientObjectResolveQueryProcessor(IImmutableDictionary<Type, IImmutableList<Expression>> retrievals)
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            _retrievals = retrievals;
        }

        public ClientObjectResolveQueryProcessor(Type type, IEnumerable<Expression> retrievals)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            _retrievals = ImmutableDictionary<Type, IImmutableList<Expression>>.Empty.Add(
                type, retrievals.ToImmutableArray()
            );
        }

        public ClientObjectResolveQueryProcessor()
        {
            _retrievals = ImmutableDictionary<Type, IImmutableList<Expression>>.Empty;
        }

        public void Include<T>(params Expression<Func<T, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            if (!retrievals.Any())
            {
                return;
            }

            _retrievals = _retrievals.SetItem(
                typeof(T),
                _retrievals.GetValueOrDefault(
                    typeof(T),
                    ImmutableList<Expression>.Empty
                )
                .AddRange(retrievals)
            );
        }

        public Expression<Func<T, Object>>[] GetRetrievals<T>()
        {
            return _retrievals
                .GetValueOrDefault(
                    typeof(T),
                    ImmutableArray<Expression>.Empty
                )
                .Cast<Expression<Func<T, Object>>>()
                .ToArray();
        }

        public Expression Process(Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var includeInjecting = new IncludeInjectingVisitor();
            var retrievalAppending = new RetrievalAppendingVisitor(_retrievals);

            var result = retrievalAppending.Visit(
                includeInjecting.Visit(
                    expression
                )
            );

            return result;
        }

        public IQueryable<T> Process<T>(IQueryable<T> query)
        {
            if (query == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(query));
            }

            return query.Provider.CreateQuery<T>(
                Process(query.Expression)
            );
        }

        private sealed class IncludeInjectingVisitor : ExpressionVisitor
        {
            protected override Expression VisitConstant(ConstantExpression node)
                => WrapWithIncludeCall(node);

            protected override Expression VisitMember(MemberExpression node)
                => WrapWithIncludeCall(node);

            protected override Expression VisitMethodCall(MethodCallExpression node)
                => WrapWithIncludeCall(node);

            private Expression WrapWithIncludeCall(Expression expression)
            {
                var elementType = ExtractQueryResultTypes(expression.Type).FirstOrDefault();

                if (elementType == null)
                {
                    return expression;
                }

                var methodCall = new IncludeCallInfo(expression as MethodCallExpression);

                if ((methodCall != null) && (methodCall.ElementType == elementType))
                {
                    return expression;
                }

                return Expression.Call(
                    null,
                    IncludeMethod.MakeGenericMethod(elementType),
                    expression,
                    Expression.NewArrayInit(
                        typeof(Expression<>).MakeGenericType(
                            typeof(Func<,>).MakeGenericType(
                                elementType,
                                typeof(Object)
                            )
                        )
                    )
                );
            }
        }

        private sealed class RetrievalAppendingVisitor : ExpressionVisitor
        {
            public RetrievalAppendingVisitor(IImmutableDictionary<Type, IImmutableList<Expression>> retrievals)
            {
                Retrievals = retrievals;
            }

            public IImmutableDictionary<Type, IImmutableList<Expression>> Retrievals
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
                var retrievals = node.Arguments[1] as NewArrayExpression;

                if (retrievals == null)
                {
                    throw Error.ArgumentOutOfRangeFormat(
                       nameof(node),
                       SR.ClientObjectResolveQueryProcessor_IncludeArgNotArray,
                       node
                   );
                }

                var retrievalsCombined = new ReadOnlyCollection<Expression>(
                    retrievals.Expressions
                    .Concat(
                        Retrievals.GetValueOrDefault(
                            retrievedType,
                            ImmutableArray<Expression>.Empty
                        )
                    )
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
        }

        private sealed class IncludeCallInfo
        {
            public IncludeCallInfo(MethodCallExpression node)
            {
                if (node == null)
                {
                    return;
                }

                if (!node.Method.DeclaringType.Equals(typeof(ClientObjectQueryableExtension)))
                {
                    return;
                }

                if (node.Arguments.Count != 2)
                {
                    return;
                }

                if (!node.Method.IsGenericMethod)
                {
                    return;
                }

                var genericArguments = node.Method.GetGenericArguments();

                if (genericArguments.Length != 1)
                {
                    return;
                }

                IsIncludeWithDefaultProperties = node.Method.Name.Equals("IncludeWithDefaultProperties");
                IsInclude = IsIncludeWithDefaultProperties || node.Method.Name.Equals("Include");
                
                if (!IsInclude)
                {
                    return;
                }

                ElementType = genericArguments[0];
            }

            public Type ElementType
            {
                get;
                private set;
            }

            public Boolean IsInclude
            {
                get;
                private set;
            }

            public Boolean IsIncludeWithDefaultProperties
            {
                get;
                private set;
            }
        }

        private static Boolean IsIncludeOrIncludeWithDefaultProperties(MethodCallExpression node)
        {
            return new IncludeCallInfo(node).IsInclude;
        }

        private static IEnumerable<Type> ExtractQueryResultTypes(Type queryable)
        {
            return from baseType in queryable.GetRuntimeBaseTypeChain()

                   from interfaceType in baseType.ImplementedInterfaces
                   where interfaceType.IsConstructedGenericType

                   let interfaceGenericDef = interfaceType.GetGenericTypeDefinition()
                   where interfaceGenericDef == typeof(IQueryable<>)

                   select interfaceType.GenericTypeArguments[0];
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ClientObjectResolveQueryProcessor>();

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