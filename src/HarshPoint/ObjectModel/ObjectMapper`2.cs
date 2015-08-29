using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ObjectModel
{
    public sealed class ObjectMapper<TSource, TTarget>
    {
        private readonly List<EntryBuilder> _entries
            = new List<EntryBuilder>();

        private readonly IDefaultValuePolicy _defaultValuePolicy;
        private readonly HarshObjectMetadata _sourceMetadata;

        public ObjectMapper()
            : this(null)
        {
        }

        public ObjectMapper(IDefaultValuePolicy defaultValuePolicy)
            : this(defaultValuePolicy, null)
        {
        }

        public ObjectMapper(
            IDefaultValuePolicy defaultValuePolicy,
            HarshObjectMetadata sourceMetadata
        )
        {
            if (defaultValuePolicy == null)
            {
                defaultValuePolicy = new NullDefaultValuePolicy();
            }

            if (sourceMetadata == null)
            {
                sourceMetadata = new HarshObjectMetadata(typeof(TSource));
            }

            _defaultValuePolicy = defaultValuePolicy;
            _sourceMetadata = sourceMetadata;
        }

        public Boolean IsEmpty => !_entries.Any();

        public IEntryBuilder Map(
            Expression<Func<TTarget, Object>> targetProperty
        )
        {
            if (targetProperty == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetProperty));
            }

            var targetPropertyName =
                targetProperty.ExtractLastPropertyAccess().Name;

            var sourceProperty =
                _sourceMetadata.GetPropertyAccessor(targetPropertyName);

            var entry = new EntryBuilder(targetProperty, sourceProperty);
            _entries.Add(entry);
            return entry;
        }

        public ObjectMapping ToMapping()
        {
            var unmapped = _entries.Where(e => e.SourceSelector == null);

            if (unmapped.Any())
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.ObjectMapper_NoSourceSelectorFound,
                    typeof(TSource),
                    String.Join(
                        ", ", unmapped.Select(e => e.TargetProperty.Name)
                    )
                );
            }

            return new ObjectMapping(
                from e in _entries
                select new ObjectMappingEntry(
                    e.TargetProperty,
                    e.SourceSelector,
                    e.SourceHasValue
                ),
                _defaultValuePolicy
            );
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ObjectMapper<,>));

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IEntryBuilder
        {
            IEntrySourceBuilder From(Func<TSource, Object> sourceSelector);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IEntrySourceBuilder
        {
            [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When")]
            void When(Func<TSource, Boolean> predicate);
        }

        private sealed class EntryBuilder : IEntryBuilder, IEntrySourceBuilder
        {
            public LambdaExpression TargetProperty { get; }
            public Func<Object, Object> SourceSelector { get; private set; }
            public Func<Object, Boolean> SourceHasValue { get; private set; }

            internal EntryBuilder(
                Expression<Func<TTarget, Object>> targetProperty,
                PropertyAccessor sourceProperty
            )
            {
                TargetProperty = targetProperty;
                SourceSelector = sourceProperty?.Getter;
            }

            public IEntrySourceBuilder From(Func<TSource, Object> sourceSelector)
            {
                if (sourceSelector == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(sourceSelector));
                }

                SourceSelector = x => sourceSelector((TSource)x);
                return this;
            }

            public void When(Func<TSource, Boolean> predicate)
            {
                if (predicate == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(predicate));
                }

                SourceHasValue = x => predicate((TSource)x);
            }
        }
    }
}
