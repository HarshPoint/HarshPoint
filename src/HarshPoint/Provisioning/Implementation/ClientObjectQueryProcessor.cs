using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed partial class ClientObjectQueryProcessor
    {
        private Int32 _maxRecursionDepth;

        private ImmutableDictionary<Type, ImmutableList<Expression>> _retrievals
           = ImmutableDictionary<Type, ImmutableList<Expression>>.Empty;

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
            => GetRetrievals(type, CreateDepthLimiter());

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

        private Expression[] GetRetrievals(Type type, DepthLimiter depthLimiter)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            if (depthLimiter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(depthLimiter));
            }

            var visitor = new RetrievalAppendingVisitor(this, depthLimiter);

            return _retrievals
                .GetValueOrDefault(type, ImmutableList<Expression>.Empty)
                .Select(visitor.Visit)
                .ToArray();
        }

        private DepthLimiter CreateDepthLimiter()
            => new DepthLimiter(MaxRecursionDepth);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ClientObjectQueryProcessor>();
    }
}