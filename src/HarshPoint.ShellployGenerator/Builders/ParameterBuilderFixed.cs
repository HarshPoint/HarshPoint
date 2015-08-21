using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderFixed : ParameterBuilder
    {
        internal ParameterBuilderFixed(Object value)
        {
            Value = value;
        }

        public Object Value { get; }

        internal override ParameterBuilder WithNext(ParameterBuilder next)
        {
            if (next?.HasElementOfType<ParameterBuilderDefaultValue>() ?? false)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ParameterBuilderFixed_AttemptedToNestDefaultValue
                );
            }

            return base.WithNext(next);
        }

        protected override void Process(ShellployCommandProperty property)
        {
            property.HasFixedValue = true;
            property.FixedValue = Value;
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderFixed));
    }
}
