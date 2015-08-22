using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class ParameterBuilder : Chain<ParameterBuilder>
    {
        private readonly String _name;
        private readonly Int32? _sortOrder;

        protected ParameterBuilder() { }

        protected ParameterBuilder(ParameterBuilder next)
            : base(next)
        {
        }

        protected ParameterBuilder(String name = null, Int32? sortOrder = null)
        {
            _name = name;
            _sortOrder = sortOrder;
        }

        public String Name => _name ?? NextElement?.Name;

        public Int32? SortOrder => _sortOrder ?? NextElement?.SortOrder;

        internal ParameterBuilder Append(ParameterBuilder other)
            => (ParameterBuilder)base.Append(other);

        internal ParameterBuilder WithNext(ParameterBuilder next)
            => (ParameterBuilder)base.WithNext(next);

        protected internal new ParameterBuilder NextElement
            => base.NextElement;

        [Obsolete]
        public virtual IEnumerable<ShellployCommandProperty> Synthesize()
        {
            if (NextElement == null)
            {
                return ImmutableArray<ShellployCommandProperty>.Empty;
            }

            var properties = NextElement.Synthesize();

            foreach (var prop in properties)
            {
                Process(prop);
            }

            return properties;
        }

        [Obsolete]
        protected virtual void Process(ShellployCommandProperty property)
        {
        }

        public Boolean HasElementsOfType<T>()
            where T : ParameterBuilder
            => Elements.OfType<T>().Any();

        public virtual ParameterBuilder InsertIntoContainer(
            ParameterBuilder existing
        )
            => (ParameterBuilder)WithNext(existing);

        protected internal abstract ParameterBuilder Accept(
            ParameterBuilderVisitor visitor
        );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilder));
    }
}
