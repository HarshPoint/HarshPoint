using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommand
    {
        public String ClassName { get; set; }
        public Boolean HasChildren { get; set; }
        public Type ParentProvisionerType { get; set; }
        public String Noun { get; internal set; }
        public Type ProvisionerType { get; set; }
        public String Verb { get; set; }
        public String Namespace { get; set; }
        public IImmutableList<ShellployCommandProperty> Properties { get; set; }
    }
}