using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class NullableBoolToNegativeSwitchVisitor :
        PropertyModelVisitor
    {
        private readonly HarshScopedValue<String> _assignedTo
            = new HarshScopedValue<String>();

        private readonly List<PropertyModel> _synthesized
            = new List<PropertyModel>();

        public override IEnumerable<PropertyModel> Visit(
            IEnumerable<PropertyModel> properties
        )
        {
            _synthesized.Clear();

            return base
                .Visit(properties)
                .Concat(_synthesized.ToImmutableArray());
        }

        protected internal override PropertyModel VisitAssignedTo(
            PropertyModelAssignedTo property
        )
        {
            using (_assignedTo.EnterIfHasNoValue(property.TargetPropertyName))
            {
                return base.VisitAssignedTo(property); 
            }
        }

        protected internal override PropertyModel VisitNoNegative(
            PropertyModelNoNegative property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            // remove the current PropertyModelNoNegative but 
            // keep the rest of the chain

            return property.NextElement; 
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (property.PropertyType == typeof(Boolean?))
            {
                var propertyName =
                    "No" + (RenamedPropertyName ?? property.Identifier);

                _synthesized.Add(
                    new PropertyModelAssignedTo(
                        _assignedTo.Value,
                        new PropertyModelNegated(
                            new PropertyModelSynthesized(
                                propertyName,
                                typeof(SMA.SwitchParameter),
                                property.Attributes
                            )
                        )
                    )
                );

                return NullableBoolToSwitch.Visit(property);
            }

            return base.VisitSynthesized(property);
        }

        private static readonly ChangePropertyTypeVisitor NullableBoolToSwitch
            = new ChangePropertyTypeVisitor(
                typeof(Boolean?), 
                typeof(SMA.SwitchParameter)
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NullableBoolToNegativeSwitchVisitor));
    }
}
