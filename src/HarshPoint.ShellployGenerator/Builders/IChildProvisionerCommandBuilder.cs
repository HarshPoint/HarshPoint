using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildProvisionerCommandBuilder
    {
        NewProvisionerCommandBuilder ParentBuilder { get; }
        Type ParentType { get; }
        PropertyModelContainer PropertyContainer { get; }
    }
}