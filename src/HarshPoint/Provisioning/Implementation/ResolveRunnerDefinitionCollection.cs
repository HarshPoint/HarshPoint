using System;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveRunnerDefinitionCollection : Collection<ResolvedProperty>
    {
        public void Add<TResult>(
            Expression<Func<IResolve<TResult>>> propertyExpression,
            Func<IResolve<TResult>> factory = null
        )
            => AddCore(propertyExpression, factory);

        public void Add<TResult>(
            Expression<Func<IResolveSingle<TResult>>> propertyExpression,
            Func<IResolveSingle<TResult>> factory = null
        )
            => AddCore(propertyExpression, factory);

        public void Add<TResult>(
            Expression<Func<IResolveSingleOrDefault<TResult>>> propertyExpression,
            Func<IResolveSingleOrDefault<TResult>> factory = null
        )
            => AddCore(propertyExpression, factory);

        private void AddCore<T>(Expression<Func<T>> propertyExpression, Func<IResolveBuilder> factory)
            where T : IResolveBuilder
        {
            if (propertyExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyExpression));
            }

            var property = propertyExpression.ExtractSinglePropertyAccess();

            Add(new ResolvedProperty(property, factory));
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveRunnerDefinitionCollection>();
    }
}
