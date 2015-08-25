using System;
using System.Linq.Expressions;

namespace HarshPoint.ObjectModel
{
    public sealed class ObjectMappingEntry
    {
        public ObjectMappingEntry(
            LambdaExpression targetExpression,
            Func<Object, Object> sourceSelector
        )
            : this(targetExpression, sourceSelector, null)
        {
        }

        public ObjectMappingEntry(
            LambdaExpression targetExpression,
            Func<Object, Object> sourceSelector,
            Func<Object, Boolean> sourceHasValue
        )
        {
            if (targetExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetExpression));
            }

            if (sourceSelector == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(sourceSelector));
            }

            TargetAccessor = new PropertyAccessor(
                targetExpression.ExtractLastPropertyAccess()
            );

            SourceSelector = sourceSelector;
            SourceHasValue = sourceHasValue;
        }

        public Func<Object, Boolean> SourceHasValue { get; }
        public Func<Object, Object> SourceSelector { get; }
        public PropertyAccessor TargetAccessor { get; }

        public override String ToString() => TargetAccessor.ToString();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ObjectMappingEntry));
    }
}
