using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelFixed : PropertyModel, IValuePropertyModel
    {
        internal PropertyModelFixed(Object value)
        {
            Value = value;
        }

        public Object Value { get; }

        public override PropertyModel InsertIntoContainer(
            PropertyModel existing
        )
        {
            PropertyModelValidator.ValidateDoesNotContain<IValuePropertyModel>(existing, this);
            return base.InsertIntoContainer(existing);
        }

        protected internal override PropertyModel Accept(
            PropertyModelVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitFixed(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelFixed));
    }
}
