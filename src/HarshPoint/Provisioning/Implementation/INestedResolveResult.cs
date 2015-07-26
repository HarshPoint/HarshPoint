using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface INestedResolveResult
    {
        ImmutableArray<Object> ExtractComponents(params TypeInfo[] componentTypes);
        Object Value { get; }
        IImmutableList<Object> Parents { get; }
    }
}
