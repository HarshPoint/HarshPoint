using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class ParameterBuilder : Chain<ParameterBuilder>
    {
        protected ParameterBuilder() { }

        protected ParameterBuilder(ParameterBuilder next)
            : base(next)
        {
            SortOrder = next?.SortOrder;
        }

        protected ParameterBuilder(Int32? sortOrder)
        {
            SortOrder = sortOrder;
        }

        public Int32? SortOrder { get; private set; }

        public ParameterBuilder Append(ParameterBuilder other)
            => (ParameterBuilder)base.Append(other);

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

        public Boolean HasElementOfType<T>()
            where T : ParameterBuilder
            => Elements.OfType<T>().Any();

        public virtual ParameterBuilder WithNextElement(ParameterBuilder next)
        {
            var result = (ParameterBuilder)WithNext(next);

            if (!result.SortOrder.HasValue)
            {
                result = result.WithSortOrder(next.SortOrder);
            }

            return result;
        }

        public ParameterBuilder WithSortOrder(Int32? sortOrder)
            => this.With(pb => pb.SortOrder = sortOrder);

        protected internal abstract ParameterBuilder Accept(
            ParameterBuilderVisitor visitor
        );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilder));
    }
}
