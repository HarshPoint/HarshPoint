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
            PropertyModelPositional propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            using (_isPositional.Enter(true))
            {
                return base.VisitPositional(propertyModel);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            if (_isPositional.Value)
            {
                var result =  new PropertyModelSynthesized(
                    propertyModel.Identifier,
                    propertyModel.PropertyType,
                    propertyModel.Attributes.Select(UpdatePosition)
                );

                _currentPosition++;

                return result;
            }

            return propertyModel;
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
