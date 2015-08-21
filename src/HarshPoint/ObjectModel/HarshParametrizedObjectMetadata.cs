using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ObjectModel
{
    public class HarshParametrizedObjectMetadata : HarshObjectMetadata
    {
        public HarshParametrizedObjectMetadata(Type type)
            : this(type, null)
        {
        }

        public HarshParametrizedObjectMetadata(
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
