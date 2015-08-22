using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class AttributeBuilder
    {
        private readonly Collection<Object> _ctorArgs
            = new Collection<Object>();

        private readonly Dictionary<String, Object> _namedArgs
            = new Dictionary<String, Object>(StringComparer.Ordinal);

        public AttributeBuilder(Type attributeType)
        {
            if (attributeType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeType));
            }

            AttributeType = attributeType;
        }

        public Type AttributeType { get; }

        public Collection<Object> Arguments => _ctorArgs;

        public Dictionary<String, Object> Properties => _namedArgs;

        public AttributeModel ToModel()
            => new AttributeModel(AttributeType, Arguments, Properties);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeBuilder));
    }
}
