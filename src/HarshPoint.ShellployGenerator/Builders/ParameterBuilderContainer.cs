using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal class ParameterBuilderContainer :
        IEnumerable<KeyValuePair<String, ParameterBuilder>>
    {
        private ImmutableDictionary<String, ParameterBuilder> _parameters
           = ImmutableDictionary.Create<String, ParameterBuilder>(
               StringComparer.Ordinal
            );

        public IEnumerator<KeyValuePair<String, ParameterBuilder>> GetEnumerator()
            => _parameters
                .OrderBy(p => p.Value.SortOrder ?? Int32.MaxValue)
                .GetEnumerator();

        public IEnumerable<KeyValuePair<String, ParameterBuilder>> ApplyTo(
            IEnumerable<KeyValuePair<String, ParameterBuilder>> other
        )
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return
                from p in other
                let ours = _parameters.GetValueOrDefault(p.Key)
                select ours == null ? p : p.WithValue(ours.Append(p.Value));
        }

        public ParameterBuilderContainer Clone()
            => (ParameterBuilderContainer)MemberwiseClone();

        public void Update(String name, ParameterBuilder parameter)
        {
            var existing = _parameters.GetValueOrDefault(name);

            if (parameter != existing)
            {
                _parameters = _parameters.SetItem(
                    name,
                    parameter.WithNext(existing)
                );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderContainer));
    }
}
