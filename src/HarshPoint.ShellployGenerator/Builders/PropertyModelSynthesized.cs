using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelSynthesized : PropertyModel
    {
        internal PropertyModelSynthesized(
            String identifier,
            Type propertyType,
            params AttributeModel[] attributes
        )
            : this(identifier, propertyType, (IEnumerable<AttributeModel>)attributes)
        {
        }

        internal PropertyModelSynthesized(
            String identifier,
            Type propertyType,
            IEnumerable<AttributeModel> attributes
        )
            : base(identifier: identifier)
        {
            if (String.IsNullOrWhiteSpace(identifier))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(identifier));
            }

            if (propertyType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyType));
            }

            if (attributes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributes));
            }

            Attributes = attributes.ToImmutableArray();
            PropertyType = propertyType;

            if (!Attributes.Any(IsParameterAttribute))
            {
                Attributes = Attributes.Add(
                    new AttributeModel(typeof(SMA.ParameterAttribute))
                );
            }
        }

        public ImmutableArray<AttributeModel> Attributes { get; }

        public Type PropertyType { get; }

        /// <summary>
        /// <see cref="PropertyModelSynthesized"/> always has to be at the 
        /// end of the chain.
        /// </summary>
        public override PropertyModel InsertIntoContainer(
            PropertyModel existing
        )
        {
            if (existing == null)
            {
                return this;
            }

            if (existing.HasElementsOfType<PropertyModelSynthesized>())
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.CommandParameterSynthesized_AttemptedToNest
                );
            }

            existing = new RemovePlaceholder().Visit(existing);

            return existing.Append(this);
        }

        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitSynthesized(this);
        }

        private static Boolean IsParameterAttribute(AttributeModel attr)
            => attr.AttributeType == typeof(SMA.ParameterAttribute);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<PropertyModelSynthesized>();

        private sealed class RemovePlaceholder : PropertyModelVisitor
        {
            internal override PropertyModel VisitIdentifiedPlaceholder(
                PropertyModelIdentifiedPlaceholder property
            )
                => Visit(property?.NextElement);
        }
    }
}
