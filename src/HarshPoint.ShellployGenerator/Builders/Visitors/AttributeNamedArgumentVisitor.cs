using System;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class AttributeNamedArgumentVisitor : PropertyModelVisitor
    {
        public AttributeNamedArgumentVisitor(
            Type attributeType, 
            String name, 
            Object value
        )
        {
            if (attributeType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeType));
            }

            if (name == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(name));
            }

            AttributeType = attributeType;
            Name = name;
            Value = value;
        }

        public Type AttributeType { get; }
        public String Name { get; }
        public Object Value { get; }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            return new PropertyModelSynthesized(
                propertyModel.Identifier,
                propertyModel.PropertyType,
                propertyModel.Attributes.Select(UpdateAttributeData)
            );
        }

        private AttributeModel UpdateAttributeData(AttributeModel data)
        {
            if (data?.AttributeType == AttributeType)
            {
                return data.SetProperty(Name, Value);
            }

            return data;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeNamedArgumentVisitor));
    }
}
