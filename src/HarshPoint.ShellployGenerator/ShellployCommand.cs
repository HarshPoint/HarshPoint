using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommand
    {
        public const String InputObjectPropertyName = "InputObject";

        public IEnumerable<String> Aliases { get; set; }
        public IImmutableList<AttributeData> Attributes { get; set; }
        public String ClassName { get; set; }
        public Boolean HasInputObject { get; set; }
        public IEnumerable<Type> ParentProvisionerTypes { get; set; }
        public String Name { get; set; }
        public Type ProvisionerType { get; set; }
        public String Namespace { get; set; }
        public IImmutableList<ShellployCommandProperty> Properties { get; set; }
        public IImmutableList<String> Usings { get; set; }
    }
}