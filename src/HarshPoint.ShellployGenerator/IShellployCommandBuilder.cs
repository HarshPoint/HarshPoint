using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal interface IShellployCommandBuilder
    {
        Type ProvisionerType { get; }
        IEnumerable<String> GetPositionalParameters(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            Boolean hasChildren
        );
        IEnumerable<ShellployCommandProperty> GetProperties(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            IImmutableDictionary<String, Int32?> positionalParametersIndices,
            Boolean hasChildren
        );
        ShellployCommand ToCommand(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        );
        IEnumerable<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        );
    }
}