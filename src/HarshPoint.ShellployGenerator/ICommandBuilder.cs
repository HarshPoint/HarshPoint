using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal interface ICommandBuilder
    {
        Type ProvisionerType { get; }

        IEnumerable<ShellployCommandProperty> GetProperties(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        );

        ShellployCommand ToCommand(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        );

        ImmutableList<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        );
    }
}