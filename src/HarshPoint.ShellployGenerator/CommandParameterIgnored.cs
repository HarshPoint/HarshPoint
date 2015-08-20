using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterIgnored : CommandParameter
    {
        internal override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray<ShellployCommandProperty>.Empty;
    }
}
