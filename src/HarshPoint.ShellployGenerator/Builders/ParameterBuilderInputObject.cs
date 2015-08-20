using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderInputObject : ParameterBuilder
    {
        public ParameterBuilderInputObject(ParameterBuilder previous)
        {
            InitializeFrom(previous);
        }

        internal override void Process(ShellployCommandProperty property)
        {
            property.IsInputObject = true;
            property.IsPositional = true;

            foreach (var attr in property.ParameterAttributes)
            {
                attr.NamedArguments["ValueFromPipeline"] = true;
            }
        }

        public static readonly String Name
            = ShellployCommand.InputObjectPropertyName;
    }
}