using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.ObjectModel
{
    public sealed class ParameterSet
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly StringComparer NameComparer
            = StringComparer.Ordinal;

        internal ParameterSet(
            String name, IEnumerable<Parameter> parameters, Boolean isDefault
        )
            : this(name, parameters, isDefault, isImplicit: false)
        {
        }

        private ParameterSet(
            String name,
            IEnumerable<Parameter> parameters,
            Boolean isDefault,
            Boolean isImplicit
        )
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (parameters == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameters));
            }

            Name = name;
            IsDefault = isDefault;
            IsImplicit = isImplicit;
            Parameters = parameters.ToImmutableArray();
        }

        public Boolean IsDefault { get; }

        public Boolean IsImplicit { get; }

        public String Name { get; }

        public IReadOnlyList<Parameter> Parameters { get; }

        public static ParameterSet CreateImplicit(
            IEnumerable<Parameter> parameters
        )
            => new ParameterSet(
                ImplicitParameterSetName,
                parameters,
                isDefault: true,
                isImplicit: true
            );

        private const String ImplicitParameterSetName
            = "__ImplicitParameterSet";

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ParameterSet>();
    }
}
