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
        {
            PrependTo(next);
        }

        protected ParameterBuilder(Int32? sortOrder)
        {
            SortOrder = sortOrder;
        }

        public Int32? SortOrder { get; private set; }

        public ParameterBuilder Append(ParameterBuilder other)
            => (ParameterBuilder)base.Append(other);

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

        protected virtual void Process(ShellployCommandProperty property)
        {
        }

        public Boolean HasElementOfType<T>()
            where T : ParameterBuilder
            => Elements.OfType<T>().Any();

        public virtual ParameterBuilder WithNext(ParameterBuilder next)
        {
            var result = this;

            if (next != null && !SortOrder.HasValue)
            {
                result = WithSortOrder(next.SortOrder);
            }

            result.PrependTo(next);
            return result;
        }

        public ParameterBuilder WithSortOrder(Int32? sortOrder)
            => this.With(pb => pb.SortOrder = sortOrder);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilder));
    }
}
