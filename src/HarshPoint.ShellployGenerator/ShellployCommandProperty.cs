using System;
using System.Collections.Generic;
using System.Linq;
using SMA = System.Management.Automation;
namespace HarshPoint.ShellployGenerator
{
    public sealed class ShellployCommandProperty
    {
        public IEnumerable<AttributeData> ParameterAttributes
            => Attributes?.Where(
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );

        public String Identifier { get; set; }
        public String PropertyName { get; set; }
        public Type Type { get; set; }
        public IReadOnlyList<AttributeData> Attributes { get; set; }
        public Type ProvisionerType { get; set; }
        public Boolean HasFixedValue { get; set; }
        public Object FixedValue { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean IsPositional { get; set; }
        public Boolean IsInputObject { get; set; }
    }
}