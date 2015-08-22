using System;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class AttributeNamedArgumentVisitor : ParameterBuilderVisitor
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

        protected internal override ParameterBuilder VisitSynthesized(ParameterBuilderSynthesized synthesizedBuilder)
            => new ParameterBuilderSynthesized(
                synthesizedBuilder.Name,
                synthesizedBuilder.ParameterType,
                synthesizedBuilder.ProvisionerType,
                synthesizedBuilder.Attributes
                    .Select(UpdateAttributeData)
                    .ToArray()
            );

        private AttributeData UpdateAttributeData(AttributeData data)
        {
            if (data?.AttributeType == AttributeType)
            {
                var result = data.Clone();
                result.NamedArguments[Name] = Value;
                return result;
            }

            return data;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeNamedArgumentVisitor));
    }
}
