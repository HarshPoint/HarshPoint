using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class CommandModel
    {
        public CommandModel(
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
            if (className == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(className));
            }

            if (name == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(name));
            }

            ClassName = className;
            Name = name;
            Namespace = @namespace;

            Aliases = NullableToImmutable(aliases);
            Attributes = NullableToImmutable(attributes);
            BaseTypes = NullableToImmutable(baseTypes);
            ImportedNamespaces = NullableToImmutable(importedNamespaces);
            Properties = NullableToImmutable(properties);
        }

        public ImmutableArray<String> Aliases { get; }
        public ImmutableArray<AttributeModel> Attributes { get; }
        public ImmutableArray<String> BaseTypes { get; }
        public String ClassName { get; }
        public ImmutableArray<String> ImportedNamespaces { get; }
        public String Name { get; }
        public String Namespace { get; }
        public ImmutableArray<PropertyModel> Properties { get; }

        private static ImmutableArray<T> NullableToImmutable<T>(
            IEnumerable<T> items
        )
        {
            if (items == null)
            {
                return ImmutableArray<T>.Empty;
            }

            return ImmutableArray.CreateRange(items);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandModel));
    }
}