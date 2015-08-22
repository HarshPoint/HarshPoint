using System;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterPositionVisitor : PropertyModelVisitor
    {
        private readonly HarshScopedValue<Boolean> _isPositional
            = new HarshScopedValue<Boolean>();

        private Int32 _currentPosition;

        protected internal override PropertyModel VisitPositional(
            PropertyModelPositional property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            using (_isPositional.Enter(true))
            {
                return base.VisitPositional(property);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            if (_isPositional.Value)
            {
                var result =  new PropertyModelSynthesized(
                    property.Identifier,
                    property.PropertyType,
                    property.Attributes.Select(UpdatePosition)
                );

                _currentPosition++;

                return result;
            }

            return property;
        }

        private AttributeModel UpdatePosition(AttributeModel attribute)
        {
            if (attribute.AttributeType == typeof(SMA.ParameterAttribute))
            {
                return attribute.SetProperty("Position", _currentPosition);
            }

            return attribute;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterPositionVisitor));
    }
}
