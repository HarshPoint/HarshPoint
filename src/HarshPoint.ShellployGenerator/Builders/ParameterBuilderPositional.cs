using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderPositional : ParameterBuilder
    {
        public ParameterBuilderPositional(Int32 sortOrder)
        {
            SortOrder = sortOrder;
        }

        internal override void Process(ShellployCommandProperty property)
        {
            property.IsPositional = true;
        }
    }
}
