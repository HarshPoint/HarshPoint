using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface INestedResolveResult
    {
        Object Value { get; }
        IImmutableList<Object> Parents { get; }
    }
}
