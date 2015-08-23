using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class PropertyModelVisitor
    {
        public IEnumerable<PropertyModel> Visit(
            IEnumerable<PropertyModel> properties
        )
        {
            if (properties == null)
            {
                return Enumerable.Empty<PropertyModel>();
            }

            return properties
                .Select(Visit)
                .Where(b => b != null)
                .ToArray();
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

        protected internal virtual PropertyModel VisitIgnored(
            PropertyModelIgnored property
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
            PropertyModelInputObject inputObjectBuilder
        )
            => VisitNext(inputObjectBuilder);

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
    }
}
