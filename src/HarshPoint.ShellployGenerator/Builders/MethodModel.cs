using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class MethodModel
    {
        public ImmutableArray<AttributeModel> Attributes { get; protected set; }
        public Boolean IsFamily { get; protected set; }
        public Boolean IsOverride { get; protected set; }
        public Boolean IsPublic { get; protected set; }
        public String Name { get; protected set; }
        public Type ReturnType { get; protected set; }
    }
}
