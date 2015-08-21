using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildCommandBuilder
    {
        ParameterBuilderContainer ParameterBuilders { get; }
        Type ParentType { get; }
    }
}