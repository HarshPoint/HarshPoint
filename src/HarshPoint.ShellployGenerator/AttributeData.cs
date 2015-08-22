using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint.ShellployGenerator
{
    public sealed class AttributeData
    {
        private readonly Collection<Object> _ctorArgs
            = new Collection<Object>();

        private readonly Dictionary<String, Object> _namedArgs
            = new Dictionary<String, Object>(StringComparer.Ordinal);

        public AttributeData(Type attributeType)
        {
            if (attributeType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeType));
            }

            AttributeType = attributeType;
        }

        public AttributeData Clone()
        {
            var clone = new AttributeData(AttributeType);
            clone.ConstructorArguments.AddRange(ConstructorArguments);
            clone.NamedArguments.AddRange(NamedArguments);
            return clone;
        }

        public Type AttributeType { get; }

        public Collection<Object> ConstructorArguments
            => _ctorArgs;

        public Dictionary<String, Object> NamedArguments
            => _namedArgs;

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeData));
    }
}
