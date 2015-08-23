using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class CommandModel
    {
        internal CommandModel(
            IEnumerable<String> aliases,
            IEnumerable<AttributeModel> attributes,
            IEnumerable<String> baseTypes,
            String className,
            IEnumerable<String> importedNamespaces,
            IEnumerable<MethodModel> methods,
            String name,
            String @namespace,
            IEnumerable<PropertyModel> properties
        )
        {
            Aliases = aliases.ToImmutableArray();
            Attributes = attributes.ToImmutableArray();
            BaseTypes = baseTypes.ToImmutableArray();
            ClassName = className;
            ImportedNamespaces = importedNamespaces.ToImmutableArray();
            Methods = methods.ToImmutableArray();
            Name = name;
            Namespace = @namespace;
            Properties = properties.ToImmutableArray();
        }

        public ImmutableArray<String> Aliases { get; }
        public ImmutableArray<AttributeModel> Attributes { get; }
        public ImmutableArray<String> BaseTypes { get; }
        public String ClassName { get; }
        public ImmutableArray<String> ImportedNamespaces { get; }
        public ImmutableArray<MethodModel> Methods { get; }
        public String Name { get; }
        public String Namespace { get; }
        public ImmutableArray<PropertyModel> Properties { get; }
    }
}