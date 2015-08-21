using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderRenamed : ParameterBuilder
    {
        internal ParameterBuilderRenamed(String propertyName)
        {
            PropertyName = propertyName;
        }

        public String PropertyName { get; }

        protected override void Process(ShellployCommandProperty property)
        {
            property.PropertyName = PropertyName;
        }

        protected internal override ParameterBuilder Accept(
            ParameterBuilderVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitRenamed(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderRenamed));
    }
}
