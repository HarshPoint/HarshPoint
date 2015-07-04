using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterSetMetadata
    {
        public static readonly String ImplicitParameterSetName = "__DefaultParameterSet";
        public static readonly StringComparer NameComparer = StringComparer.Ordinal;

        public ParameterSetMetadata(String name, IEnumerable<ParameterMetadata> parameters, Boolean isDefault)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(name));
            }

            if (parameters == null)
            {
                throw Error.ArgumentNull(nameof(parameters));
            }

            Name = name;
            IsDefault = isDefault;
            Parameters = parameters.ToImmutableArray();
        }

        public Boolean IsDefault
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public IReadOnlyList<ParameterMetadata> Parameters
        {
            get;
            private set;
        }
    }
}
