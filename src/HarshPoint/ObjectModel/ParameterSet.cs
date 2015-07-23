using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ObjectModel
{
    internal sealed class ParameterSet
    {
        public static readonly String ImplicitParameterSetName = "__DefaultParameterSet";
        public static readonly StringComparer NameComparer = StringComparer.Ordinal;

        public ParameterSet(String name, IEnumerable<Parameter> parameters, Boolean isDefault)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentOutOfRange_NullOrWhitespace(nameof(name));
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

        public IReadOnlyList<Parameter> Parameters
        {
            get;
            private set;
        }
    }
}
