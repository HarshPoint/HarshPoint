using System;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterDefaultValue : CommandParameter
    {
        internal CommandParameterDefaultValue(Object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public Object DefaultValue { get; }

        internal override void Process(ShellployCommandProperty property)
        {
            property.DefaultValue = DefaultValue;
        }
    }
}
