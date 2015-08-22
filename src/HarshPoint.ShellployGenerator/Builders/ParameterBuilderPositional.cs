using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderPositional : ParameterBuilder
    {
        public ParameterBuilderPositional(Int32 sortOrder)
            : base(sortOrder: sortOrder)
        {
        }

        protected override void Process(ShellployCommandProperty property)
        {
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

            return visitor.VisitPositional(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderPositional));
    }
}
