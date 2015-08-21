using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderContainer :
        IEnumerable<KeyValuePair<String, ParameterBuilder>>
    {
        private ImmutableDictionary<String, ParameterBuilder> _parameters
           = ImmutableDictionary.Create<String, ParameterBuilder>(
               StringComparer.Ordinal
            );

        private Int32 _nextPositionalParam;

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

        public IEnumerator<KeyValuePair<String, ParameterBuilder>> GetEnumerator()
            => _parameters
                .OrderBy(p => p.Value.SortOrder ?? Int32.MaxValue)
                .GetEnumerator();

        public ParameterBuilderFactory<TTarget> GetFactory<TTarget>(
            Expression<Func<TTarget, Object>> expression,
            Boolean isPositional = false
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var name = expression.ExtractLastPropertyAccess().Name;

            if (isPositional)
            {
                SetPositional(name);
            }

            return new ParameterBuilderFactory<TTarget>(this, name);
        }

        public ParameterBuilderFactory GetFactory(
            String name,
            Boolean isPositional = false
        )
        {
            CommandBuilder.ValidateParameterName(name);

            if (isPositional)
            {
                SetPositional(name);
            }

            return new ParameterBuilderFactory(this, name);
        }

        private void SetPositional(String name)
        {
            Update(
                name,
                new ParameterBuilderPositional(_nextPositionalParam)
            );

            _nextPositionalParam++;
        }

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
