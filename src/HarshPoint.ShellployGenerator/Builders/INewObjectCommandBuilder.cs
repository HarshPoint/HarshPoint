using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface INewObjectCommandBuilder
    {
        IEnumerable<PropertyModel> CreatePropertiesRecursively();
        IImmutableList<Type> ParentTargetTypes { get; }
        Type TargetType { get; }
    }
}