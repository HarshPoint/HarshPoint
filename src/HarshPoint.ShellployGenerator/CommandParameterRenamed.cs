using System;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterRenamed : CommandParameter
    {
        internal CommandParameterRenamed(String propertyName)
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
