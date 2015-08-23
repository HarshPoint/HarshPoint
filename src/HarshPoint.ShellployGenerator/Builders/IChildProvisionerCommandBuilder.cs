using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildProvisionerCommandBuilder
    {
        PropertyModelContainer PropertyContainer { get; }
        Type ParentType { get; }
    }
}