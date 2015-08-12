using System;

namespace HarshPoint.ShellployGenerator
{
    internal interface IShellployMetadataObject
    {
        IShellployCommandBuilder GetCommandBuilder();
        Type GetProvisionerType();
    }
}
