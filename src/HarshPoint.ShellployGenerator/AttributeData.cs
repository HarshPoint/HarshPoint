using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class AttributeData
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

        public Type AttributeType { get; }

        public Collection<Object> ConstructorArguments
            => _ctorArgs;

        public Dictionary<String, Object> NamedArguments
            => _namedArgs;

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeData));
    }
}
