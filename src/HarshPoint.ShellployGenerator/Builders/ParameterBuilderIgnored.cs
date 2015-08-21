using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderIgnored : ParameterBuilder
    {
        public override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray<ShellployCommandProperty>.Empty;
    }
}
