using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    internal struct LazyObjectMapping<TSource, TTarget>
    {
        private ObjectMapper<TSource, TTarget> _mapper;
        private ObjectMapping _mapping;
        private HarshObjectMetadata _sourceMetadata;

        public LazyObjectMapping(HarshObjectMetadata sourceMetadata)
        {
            _mapper = null;
            _mapping = null;
            _sourceMetadata = sourceMetadata;
        }

        public IEnumerable<ObjectMappingAction> Apply(
            Object source,
            Object target
        )
        {
            if (HasEntries)
            {
                return Mapping.Apply(source, target);
            }

            return Enumerable.Empty<ObjectMappingAction>();
        }

        public Boolean Apply<TContext>(
            RecordWriter<TContext, TTarget> writeRecord,
            Object source,
            TTarget target
        )
            where TContext : HarshProvisionerContextBase<TContext>
            => Apply(writeRecord, null, source, target);

        public Boolean Apply<TContext>(
            RecordWriter<TContext, TTarget> writeRecord,
            String context,
            Object source,
            TTarget target
        )
            where TContext : HarshProvisionerContextBase<TContext>
        {
            if (writeRecord == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(writeRecord));
            }

            var result = false;

            foreach (var a in Apply(source, target))
            {
                if (a.ValuesEqual)
                {
                    writeRecord.PropertyUnchanged(
                        context,
                        a.TargetAccessor.Name,
                        target,
                        a.TargetValue
                    );
                }
                else
                {
                    result = true;
                    writeRecord.PropertyChanged(
                        context,
                        a.TargetAccessor.Name,
                        target,
                        a.TargetValue,
                        a.SourceValue
                    );
                }

            }

            return result;
        }

        public IEnumerable<ObjectMappingAction> GetActions(
            Object source,
            Object target
        )
        {
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (HasEntries)
            {
                return Mapping.GetActions(source, target);
            }

            return Enumerable.Empty<ObjectMappingAction>();
        }

        public Expression<Func<TTarget, Object>>[] GetTargetExpressions()
        {
            if (HasEntries)
            {
                return Mapping.GetTargetExpressions<TTarget>().ToArray();
            }

            return new Expression<Func<TTarget, Object>>[0];
        }

        public ObjectMapper<TSource, TTarget>.IEntryBuilder Map(
            Expression<Func<TTarget, Object>> targetProperty
        )
            => Mapper.Map(targetProperty);

        public Boolean WouldChange(Object source, Object target)
        {
            if (source == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(source));
            }

            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (HasEntries)
            {
                return Mapping.WouldChange(source, target);
            }

            return false;
        }

        private Boolean HasEntries => _mapper != null && !_mapper.IsEmpty;

        private ObjectMapper<TSource, TTarget> Mapper
        {
            get
            {
                var sourceMetadata = _sourceMetadata;

                return HarshLazy.Initialize(ref _mapper, () =>
                    new ObjectMapper<TSource, TTarget>(
                        ProvisioningDefaultValuePolicy.Instance,
                        sourceMetadata
                    )
                );
            }
        }

        private ObjectMapping Mapping
        {
            get
            {
                if (_mapping == null && HasEntries)
                {
                    _mapping = _mapper.ToMapping();
                }

                return _mapping;
            }
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(LazyObjectMapping<,>));
    }
}
