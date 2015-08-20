using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

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
        public String Noun { get; set; }
        public Type ProvisionerType { get; set; }
        public Tuple<Type, String> Verb { get; set; }
        public String Namespace { get; set; }
        public IImmutableList<ShellployCommandProperty> Properties { get; set; }
        public Type ContextType { get; set; }
        public IImmutableList<String> Usings { get; set; }
    }
}