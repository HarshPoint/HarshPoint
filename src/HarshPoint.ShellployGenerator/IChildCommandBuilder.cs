using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator
{
    internal interface IChildCommandBuilder
    {
        IEnumerable<ShellployCommandProperty> Process(
            IEnumerable<ShellployCommandProperty> parentProperties
        );
        Type Type { get; }
    }
}