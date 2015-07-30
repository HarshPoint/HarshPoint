using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator
{
    internal interface IShellployCommandBuilderParent
    {
        Dictionary<String, object> FixedParameters { get; }
        Type Type { get; }
    }
}