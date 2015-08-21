using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderIgnored : ParameterBuilder
    {
        public override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray<ShellployCommandProperty>.Empty;

        protected internal override ParameterBuilder Accept(
            ParameterBuilderVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitIgnored(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderIgnored));
    }
}
