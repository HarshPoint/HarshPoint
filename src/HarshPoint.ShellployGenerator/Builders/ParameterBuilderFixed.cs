using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderFixed : ParameterBuilder
    {
        internal ParameterBuilderFixed(Object value)
        {
            Value = value;
        }

        public Object Value { get; }

        public override ParameterBuilder WithNextElement(ParameterBuilder next)
        {
            if (next?.HasElementOfType<ParameterBuilderDefaultValue>() ?? false)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ParameterBuilderFixed_AttemptedToNestDefaultValue
                );
            }

            return base.WithNextElement(next);
        }

        protected override void Process(ShellployCommandProperty property)
        {
            property.HasFixedValue = true;
            property.FixedValue = Value;
        }

        protected internal override ParameterBuilder Accept(ParameterBuilderVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitFixed(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderFixed));
    }
}
