using System;
using System.Collections.Generic;

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

        IEnumerable<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        );
    }
}