using System;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal interface IShellployCommandBuilderParent
    {
        ImmutableDictionary<String, Object> FixedParameters { get; }
        ImmutableHashSet<String> IgnoredParameters { get; }
        Type Type { get; }
    }
}