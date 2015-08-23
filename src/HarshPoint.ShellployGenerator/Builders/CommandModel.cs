using System;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class CommandModel
    {
        public ImmutableArray<String> Aliases { get; set; }
        public ImmutableArray<AttributeModel> Attributes { get; set; }
        public ImmutableArray<String> BaseTypes { get; set; }
        public String ClassName { get; set; }
        public ImmutableArray<String> ImportedNamespaces { get; set; }
        public String Name { get; set; }
        public String Namespace { get; set; }
        public ImmutableArray<PropertyModel> Properties { get; set; }
    }
}