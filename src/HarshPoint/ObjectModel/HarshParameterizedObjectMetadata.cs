using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ObjectModel
{
    public class HarshParameterizedObjectMetadata : HarshObjectMetadata
    {
        public HarshParameterizedObjectMetadata(Type type)
            : this(type, null)
        {
        }

        public HarshParameterizedObjectMetadata(
            Type type, 
            IDefaultValuePolicy defaultValuePolicy
        )
            : base(type)
        {
            ParameterSets = new ParameterSetBuilder(this, defaultValuePolicy)
                .Build()
                .ToImmutableArray();

            PropertyParameters = Parameters.ToLookup(p => p.PropertyAccessor);

            DefaultParameterSet = ParameterSets.SingleOrDefault(
                set => set.IsDefault
            );
        }

        public ParameterSet DefaultParameterSet { get; }

        public IEnumerable<Parameter> Parameters
            => ParameterSets.SelectMany(set => set.Parameters);

        public IEnumerable<ParameterSet> ParameterSets { get; }

        public IEnumerable<PropertyAccessor> ParameterProperties
            => PropertyParameters.Select(g => g.Key);

        public ILookup<PropertyAccessor, Parameter> PropertyParameters { get; }
    }
}
