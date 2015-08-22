using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelDefaultValue : PropertyModel
    {
        internal PropertyModelDefaultValue(Object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public Object DefaultValue { get; }

        public override PropertyModel InsertIntoContainer(
            PropertyModel existing
        )
        {
            if ((existing != null) &&
                (existing.HasElementsOfType<PropertyModelFixed>()))
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ParameterBuilderDefaultValue_AttemptedToNestFixed
                );
            }

            return base.InsertIntoContainer(existing);
        }

        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitDefaultValue(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelDefaultValue));
    }
}
