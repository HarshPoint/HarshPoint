using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class MethodModelNewObject : MethodModel
    {
        public MethodModelNewObject(
            Type targetType, 
            IEnumerable<PropertyModel> properties
        )
        {
            Name = $"New{targetType.Name}";
            ReturnType = targetType;
            Properties = properties.ToImmutableArray();
        }

        public ImmutableArray<PropertyModel> Properties { get; }
    }
}
