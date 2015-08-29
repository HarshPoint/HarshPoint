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
            PropertyModelAssignedTo propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            using (_assignedTo.EnterIfHasNoValue(propertyModel.TargetPropertyName))
            {
                return base.VisitAssignedTo(propertyModel); 
            }
        }

        protected internal override PropertyModel VisitNoNegative(
            PropertyModelNoNegative propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            // remove the current PropertyModelNoNegative but 
            // keep the rest of the chain

            return propertyModel.NextElement; 
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            if (propertyModel.PropertyType == typeof(Boolean?))
            {
                var positivePropertyName = 
                    (RenamedPropertyName ?? propertyModel.Identifier);

                var propertyName = "No" + positivePropertyName;

                _synthesized.Add(
                    new PropertyModelAssignedTo(
                        _assignedTo.Value,
                        new PropertyModelNegated(
                            positivePropertyName,
                            new PropertyModelSynthesized(
                                propertyName,
                                typeof(SMA.SwitchParameter),
                                propertyModel.Attributes
                            )
                        )
                    )
                );

                return NullableBoolToSwitch.Visit(propertyModel);
            }

            return base.VisitSynthesized(propertyModel);
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
