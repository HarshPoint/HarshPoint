using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class PropertyModelVisitor : IVisitor<PropertyModel>
    {
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
    }
}
