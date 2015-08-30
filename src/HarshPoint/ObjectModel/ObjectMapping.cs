using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ObjectModel
{
    public sealed class ObjectMapping
    {
        public ObjectMapping(IEnumerable<ObjectMappingEntry> entries)
            : this(entries, null)
        {
        }

        public ObjectMapping(
            IEnumerable<ObjectMappingEntry> entries,
            IDefaultValuePolicy defaultValuePolicy
        )
        {
            if (entries == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(entries));
            }

            DefaultValuePolicy =
                defaultValuePolicy ?? new NullDefaultValuePolicy();

            Entries = entries.ToImmutableArray();
        }

        public IEnumerable<ObjectMappingAction> Apply(Object source, Object target)
        {
            var actions = GetActions(source, target).ToImmutableArray();

            foreach (var a in actions.Where(a => !a.ValuesEqual))
            {
                a.TargetAccessor.SetValue(
                    target,
                    a.SourceValue
                );
            }

            return actions;
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

            return from e in Entries
                   where HasSourceValue(e, source)

                   let sourceValue = e.SourceSelector(source)
                   where !IsSourceValueDefault(e, sourceValue)

                   let targetValue = e.TargetAccessor.GetValue(
                       target
                   )

                   select new ObjectMappingAction(
                       e.TargetAccessor,
                       sourceValue,
                       targetValue
                   );
        }

        public IEnumerable<Expression<Func<T, Object>>> GetTargetExpressions<T>()
            => Entries.Select(
                m => m.TargetAccessor.MakeGetterExpression<T, Object>()
            );

        public Boolean WouldChange(Object source, Object target)
            => GetActions(source, target).Any(a => !a.ValuesEqual);

        public IDefaultValuePolicy DefaultValuePolicy { get; }

        public IImmutableList<ObjectMappingEntry> Entries { get; }

        private Boolean IsSourceValueDefault(
            ObjectMappingEntry e,
            Object sourceValue
        )
        {
            if (DefaultValuePolicy.IsDefaultValue(sourceValue))
            {
                Logger.Information(
                    "Default value {$Value} set for {TargetProperty}, skipping.",
                    sourceValue,
                    e
                );

                return true;
            }

            return false;
        }

        private static Boolean HasSourceValue(
            ObjectMappingEntry e,
            Object source
        )
        {
            if (e.SourceHasValue == null)
            {
                return true;
            }

            if (!e.SourceHasValue(source))
            {
                Logger.Information(
                    "No value set for {TargetProperty}, skipping.",
                    e
                );

                return false;
            }

            return true;
        }

        private static Boolean ValuesEqual(
            ObjectMappingEntry e,
            Object sourceValue,
            Object targetValue
        )
        {
            if (Equals(sourceValue, targetValue))
            {
                Logger.Information(
                    "Source value {$SourceValue} equals to the target object " +
                    "{TargetProperty} value {$TargetValue}.",
                    sourceValue,
                    e.TargetAccessor,
                    targetValue
                );

                return true;
            }

            return false;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ObjectMapping));
    }
}
