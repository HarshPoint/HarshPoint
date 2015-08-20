using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildCommandBuilder
    {
        IEnumerable<ShellployCommandProperty> Process(
            IEnumerable<ShellployCommandProperty> parentProperties
        );
        Type ProvisionerType { get; }
    }
}