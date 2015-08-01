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
    internal sealed class ClientObjectQueryProcessor
    {
        private IImmutableDictionary<Type, IImmutableList<Expression>> _retrievals
           = ImmutableDictionary<Type, IImmutableList<Expression>>.Empty;

        public void Include<T>(params Expression<Func<T, Object>>[] retrievals)
        {
            if (retrievals == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(retrievals));
            }

            Logger.Debug(
                "Including {Type} {@Retrievals}",
                typeof(T),
                retrievals
            );

            if (!retrievals.Any())
            {
                return;
            }

            var retrievalsWithIncludes = IncludeInjectingVisitor.Instance.Visit(
                new ReadOnlyCollection<Expression>(retrievals)
            );

            Logger.Debug(
                "Retrievals with includes for {Type}: {@Retrievals}",
                typeof(T),
                retrievalsWithIncludes
            );

            _retrievals = _retrievals.SetItem(
                typeof(T),
                _retrievals.GetValueOrDefault(
                    typeof(T),
                    ImmutableList<Expression>.Empty
                )
                .AddRange(retrievalsWithIncludes)
            );
        }

        public Expression<Func<T, Object>>[] GetRetrievals<T>()
            => GetRetrievals(typeof(T))
                .Cast<Expression<Func<T, Object>>>()
                .ToArray();

        public Expression[] GetRetrievals(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            var visitor = new RetrievalAppendingVisitor(this);

            return _retrievals
                .GetValueOrDefault(
                    type,
                    ImmutableArray<Expression>.Empty
                )
                .Select(visitor.Visit)
                .ToArray();
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

        public Expression Process(Expression expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            Logger.Debug("Expression processing: {Expression}", expression);

            var includesInjected = IncludeInjectingVisitor.Instance.Visit(expression);
            Logger.Debug("Includes injected: {Expression}", includesInjected);

            var retrievalAppending = new RetrievalAppendingVisitor(this);
            var result = retrievalAppending.Visit(includesInjected);
            Logger.Debug("Retrievals appended: {Expression}", result);

            return result;
        }

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
                var elementType = ExtractQueryResultTypes(expression.Type).FirstOrDefault();

                if (elementType == null)
                {
                    return expression;
                }

                var instance = expression;
                var retrievals = new ReadOnlyCollection<Expression>(new Expression[0]);

                var methodCall = expression as MethodCallExpression;
                var includeCall = new IncludeCallInfo(methodCall);

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

            public static IncludeInjectingVisitor Instance { get; }
                = new IncludeInjectingVisitor();
        }

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

                var callInfo = new IncludeCallInfo(node);

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

        private static IEnumerable<Type> ExtractQueryResultTypes(Type queryable)
        {
            if (queryable.IsConstructedGenericType &&
                queryable.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                return ImmutableArray.Create(queryable.GenericTypeArguments[0]);
            }

            return from baseType in queryable.GetRuntimeBaseTypeChain()

                   from interfaceType in baseType.ImplementedInterfaces
                   where interfaceType.IsConstructedGenericType

                   let interfaceGenericDef = interfaceType.GetGenericTypeDefinition()
                   where interfaceGenericDef == typeof(IQueryable<>)

                   select interfaceType.GenericTypeArguments[0];
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ClientObjectQueryProcessor>();

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