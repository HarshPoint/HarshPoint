using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class PropertyModelVisitor
    {
        private readonly HarshScopedValue<String> _renamed
            = new HarshScopedValue<String>();

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

        public virtual PropertyModel Visit(PropertyModel propertyModel)
        {
            if (propertyModel != null)
            {
                return propertyModel.Accept(this);
            }

            return null;
        }

        protected internal virtual PropertyModel VisitDefaultValue(
            PropertyModelDefaultValue propertyModel
        )
            => VisitNext(propertyModel);

        internal virtual PropertyModel VisitIdentifiedPlaceholder(
            PropertyModelIdentifiedPlaceholder propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitIgnored(
            PropertyModelIgnored propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitNegated(
            PropertyModelNegated propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitNoNegative(
            PropertyModelNoNegative propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitRenamed(
            PropertyModelRenamed propertyModel
        )
        {
            if (propertyModel == null)
            {
                return null;
            }

            using (_renamed.EnterIfHasNoValue(propertyModel.PropertyName))
            {
                return VisitNext(propertyModel);
            }
        }

        protected internal virtual PropertyModel VisitPositional(
            PropertyModelPositional propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitInputObject(
            PropertyModelInputObject propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitAssignedTo(
            PropertyModelAssignedTo propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitFixed(
            PropertyModelFixed propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitConditionalFixed(
            PropertyModelConditionalFixed propertyModel
        )
            => VisitNext(propertyModel);

        protected internal virtual PropertyModel VisitSynthesized(
            PropertyModelSynthesized propertyModel
        )
            => VisitNext(propertyModel);

        protected String RenamedPropertyName => _renamed.Value;

        private PropertyModel VisitNext(PropertyModel propertyModel)
        {
            if (propertyModel?.NextElement != null)
            {
                return propertyModel.WithNext(
                    Visit(propertyModel.NextElement)
                );
            }

            return propertyModel;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelVisitor));
    }
}
