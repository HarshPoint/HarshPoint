using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderRenamed : ParameterBuilder
    {
        internal ParameterBuilderRenamed(String propertyName)
        {
            PropertyName = propertyName;
        }

        public String PropertyName { get; }

        internal override void Process(ShellployCommandProperty property)
        {
            property.PropertyName = PropertyName;
        }
    }
}
