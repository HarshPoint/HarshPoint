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

        internal override void Process(ShellployCommandProperty property)
        {
            property.HasFixedValue = true;
            property.FixedValue = Value;
        }
    }
}
