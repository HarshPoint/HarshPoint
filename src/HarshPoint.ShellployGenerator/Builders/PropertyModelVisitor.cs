using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class PropertyModelVisitor
    {
        public virtual IEnumerable<PropertyModel> Visit(
            IEnumerable<PropertyModel> properties
        )
        {
            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            return properties
                .Select(Visit)
                .Where(result => result != null)
                .ToImmutableArray();
        }

        public virtual PropertyModel Visit(PropertyModel property)
        {
            if (property != null)
            {
                return property.Accept(this);
            }

            return null;
        }

        protected internal virtual PropertyModel VisitDefaultValue(
            PropertyModelDefaultValue property
        )
            => VisitNext(property);

        internal virtual PropertyModel VisitIdentifiedPlaceholder(
            PropertyModelIdentifiedPlaceholder property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitIgnored(
            PropertyModelIgnored property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitNoNegative(
            PropertyModelNoNegative property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitRenamed(
            PropertyModelRenamed property
        )
            => VisitNext(property);


        protected internal virtual PropertyModel VisitPositional(
            PropertyModelPositional property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitInputObject(
            PropertyModelInputObject property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitAssignedTo(
            PropertyModelAssignedTo property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitFixed(
            PropertyModelFixed property
        )
            => VisitNext(property);

        protected internal virtual PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
            => VisitNext(property);

        private PropertyModel VisitNext(PropertyModel property)
        {
            if (property?.NextElement != null)
            {
                return property.WithNext(
                    Visit(property.NextElement)
                );
            }

            return property;
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelVisitor));
    }
}
