using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderContainer :
        IEnumerable<ParameterBuilder>
    {
        private ImmutableDictionary<String, ParameterBuilder> _parameters
           = ImmutableDictionary.Create<String, ParameterBuilder>(
               StringComparer.Ordinal
            );

        private Int32 _nextPositionalParam;

        public IEnumerable<ParameterBuilder> ApplyTo(
            IEnumerable<ParameterBuilder> other
        )
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return from p in other
                   let ours = _parameters.GetValueOrDefault(p.Name)
                   select ours?.Append(p) ?? p;
        }

        public ParameterBuilderContainer Clone()
            => (ParameterBuilderContainer)MemberwiseClone();

        public IEnumerator<ParameterBuilder> GetEnumerator()
            => _parameters
                .Values
                .OrderBy(p => p.SortOrder ?? Int32.MaxValue)
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
                    parameter.InsertIntoContainer(existing)
                );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderContainer));
    }
}
