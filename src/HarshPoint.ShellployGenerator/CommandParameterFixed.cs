using System;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterFixed : CommandParameter
    {
        internal CommandParameterFixed(Object value)
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
