using System.Linq;
using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommandProperty
    {
        public String Name { get; set; }
        public Type Type { get; set; }
        public IImmutableList<ShellployCommandPropertyParameterAttribute> ParameterAttributes { get; set; }
        public Type AssignmentOnType { get; set; }
        public Boolean UseFixedValue { get; set; }
        public Object FixedValue { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean Custom { get; set; }
    }
}