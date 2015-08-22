using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildCommandBuilder
    {
        PropertyModelContainer ParameterBuilders { get; }
        Type ParentType { get; }
    }
}