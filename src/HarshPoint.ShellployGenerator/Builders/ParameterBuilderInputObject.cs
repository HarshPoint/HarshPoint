using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderInputObject : ParameterBuilder
    {
        public ParameterBuilderInputObject(ParameterBuilder next)
            : base(next)
        {
        }

        protected override void Process(ShellployCommandProperty property)
        {
            property.IsInputObject = true;
            property.IsPositional = true;
        }
    }
}