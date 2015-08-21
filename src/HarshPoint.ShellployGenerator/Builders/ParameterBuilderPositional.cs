using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderPositional : ParameterBuilder
    {
        public ParameterBuilderPositional(Int32 sortOrder)
            : base(sortOrder)
        {
        }

        protected override void Process(ShellployCommandProperty property)
        {
            property.IsPositional = true;
        }
    }
}
