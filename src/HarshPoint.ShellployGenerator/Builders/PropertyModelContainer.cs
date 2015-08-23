using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class PropertyModelContainer : IEnumerable<PropertyModel>
    {
        private readonly CommandBuilder _owner;

        private ImmutableDictionary<String, PropertyModel> _parameters
           = ImmutableDictionary.Create<String, PropertyModel>(
               StringComparer.Ordinal
            );

        private Int32 _nextPositionalParam;

        public PropertyModelContainer(CommandBuilder owner)
        {
            if (owner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(owner));
            }

            _owner = owner;
        }

        public IEnumerable<PropertyModel> ApplyTo(
            IEnumerable<PropertyModel> other
        )
        {
            if (other == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(other));
            }

            return from p in other
                   let ours = _parameters.GetValueOrDefault(p.Identifier)
                   select ours?.Append(p) ?? p;
        }

        public IEnumerator<PropertyModel> GetEnumerator()
            => _parameters
                .Values
                .OrderBy(p => p.SortOrder ?? Int32.MaxValue)
                .GetEnumerator();

        public ParameterBuilder<TTarget> GetParameterBuilder<TTarget>(
            Expression<Func<TTarget, Object>> expression,
            Boolean isPositional = false
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var name = expression.ExtractLastPropertyAccess().Name;

            ValidatePropertyName(name);

            if (isPositional)
            {
                SetPositional(name);
            }

            return new ParameterBuilder<TTarget>(this, name);
        }

        public ParameterBuilder GetParameterBuilder(
            String name,
            Boolean isPositional = false
        )
        {
            ValidatePropertyName(name);

            if (isPositional)
            {
                SetPositional(name);
            }

            return new ParameterBuilder(this, name);
        }

        public void ValidatePropertyName(String name)
        {
            _owner.ValidatePropertyName(name);
        }

        private void SetPositional(String name)
        {
            Update(
                name,
                new PropertyModelPositional(_nextPositionalParam)
            );

            _nextPositionalParam++;
        }

        public void Update(String name, PropertyModel parameter)
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
            = HarshLog.ForContext(typeof(PropertyModelContainer));
    }
}
