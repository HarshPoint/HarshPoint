using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderInputObject : ParameterBuilder
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

        protected internal override ParameterBuilder Accept(
            ParameterBuilderVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitInputObject(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderInputObject));
    }
}