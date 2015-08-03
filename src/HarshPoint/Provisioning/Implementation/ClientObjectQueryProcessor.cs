using HarshPoint.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed partial class ClientObjectQueryProcessor
    {
        private Int32 _maxRecursionDepth;

        private RetrievalAppendingVisitor _retrievalAppender;

        private ImmutableDictionary<Type, ImmutableHashSet<Expression>> _retrievals
           = ImmutableDictionary<Type, ImmutableHashSet<Expression>>.Empty;

        public ClientObjectQueryProcessor()
        {
            _retrievalAppender = new RetrievalAppendingVisitor(this);
        }

        public Int32 MaxRecursionDepth
        {
            get { return _maxRecursionDepth; }
            set
            {
                if (value < 0)
                {
                    throw Logger.Fatal.ArgumentOutOfRange(
                        nameof(value),
                        SR.ClientObjectResolveQueryProcessor_MaxRecursionDepthNegative
                    );
                }

                _maxRecursionDepth = value;
            }
        }

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
                    CreateExpressionHashSet()
                )
                .Union(retrievalsWithIncludes)
            );
        }

        public Expression<Func<T, Object>>[] GetRetrievals<T>()
            => GetRetrievalsRecursive(typeof(T))
                .Cast<Expression<Func<T, Object>>>()
                .ToArray();

        public Expression[] GetRetrievals(Type type)
            => GetRetrievalsRecursive(type)
                .ToArray();

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

            var result = _retrievalAppender.Visit(includesInjected);
            Logger.Debug("Retrievals appended: {Expression}", result);

            return result;
        }

        private IEnumerable<Expression> GetRetrievalsRecursive(Type type)
            => GetRetrievalsCore(type).Select(_retrievalAppender.Visit);

        private IEnumerable<Expression> GetRetrievalsCore(Type type)
            => _retrievals.GetValueOrDefault(
                type,
                CreateExpressionHashSet()
            );

        private static ImmutableHashSet<Expression> CreateExpressionHashSet()
            => ImmutableHashSet.Create<Expression>(
                HarshExpressionEqualityComparer.Instance
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ClientObjectQueryProcessor>();
    }
}