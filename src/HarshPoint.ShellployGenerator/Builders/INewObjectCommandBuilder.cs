using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface INewObjectCommandBuilder
    {
        ImmutableList<KeyValuePair<String, ParameterBuilder>> GetParametersRecursively();
        IImmutableList<Type> ParentTargetTypes { get; }
        Type TargetType { get; }
    }
}