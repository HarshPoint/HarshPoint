using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderDefaultValue : ParameterBuilder
    {
        internal ParameterBuilderDefaultValue(Object defaultValue)
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
