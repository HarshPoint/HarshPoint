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
            Name = name;
            Namespace = @namespace;
            Properties = properties.ToImmutableArray();
        }

        public IImmutableList<String> Aliases { get; }
        public IImmutableList<AttributeModel> Attributes { get; }
        public IImmutableList<String> BaseTypes { get; }
        public String ClassName { get; }
        public IImmutableList<String> ImportedNamespaces { get; }
        public String Name { get; }
        public String Namespace { get; }
        public IImmutableList<PropertyModel> Properties { get; }
    }
}