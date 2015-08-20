using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class ShellployCommandProperty
    {
        public String Identifier { get; set; }
        public String PropertyName { get; set; }
        public Type Type { get; set; }
        public IReadOnlyList<AttributeData> Attributes { get; set; }
        public Type ProvisionerType { get; set; }
        public Boolean HasFixedValue { get; set; }
        public Object FixedValue { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean IsPositional { get; set; }
    }
}