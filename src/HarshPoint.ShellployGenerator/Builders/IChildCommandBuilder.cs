using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildCommandBuilder
    {
        ParameterBuilderContainer Parameters { get; }
        Type ProvisionerType { get; }
    }
}