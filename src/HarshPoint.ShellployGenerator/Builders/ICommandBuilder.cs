using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface ICommandBuilder
    {
        Type ProvisionerType { get; }

        ImmutableList<Type> GetParentProvisionerTypes(
            CommandBuilderContext context
        );

        IEnumerable<ShellployCommandProperty> GetProperties(
            CommandBuilderContext context
        );

        ShellployCommand ToCommand(CommandBuilderContext context);
    }
}