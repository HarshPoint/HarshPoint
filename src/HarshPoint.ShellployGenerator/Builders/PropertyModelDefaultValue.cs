using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelDefaultValue : PropertyModel, IValuePropertyModel
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
            PropertyModelValidator.ValidateDoesNotContain<IValuePropertyModel>(existing, this);
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
