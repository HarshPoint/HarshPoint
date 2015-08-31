using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class AttributeModel : IHarshCloneable
    {
        private IImmutableList<Object> _arguments;
        private IImmutableDictionary<String, Object> _properties;

        public AttributeModel(Type attributeType)
        {
            if (attributeType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeType));
            }

            AttributeType = attributeType;

            _arguments = ImmutableList.Create<Object>();
            _properties = ImmutableDictionary.Create<String, Object>(
                StringComparer.Ordinal
            );
        }

        public AttributeModel(
            Type attributeType,
            IEnumerable<Object> arguments,
            IEnumerable<KeyValuePair<String, Object>> properties
        )
        {
            if (arguments == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(arguments));
            }

            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            AttributeType = attributeType;

            _arguments = arguments.ToImmutableList();
            _properties = properties.ToImmutableDictionary();
        }

        public Type AttributeType { get; }

        public IReadOnlyList<Object> Arguments => _arguments;

        public IReadOnlyDictionary<String, Object> Properties => _properties;

        public AttributeModel AddArgument(Object value)
            => this.With(am => am._arguments = am._arguments.Add(value));

        public AttributeModel RemoveProperty(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return this.With(
                am => am._properties = am._properties.Remove(name)
            );
        }

        public AttributeModel SetArgument(Int32 index, Object value)
            => this.With(
                am => am._arguments = am._arguments.SetItem(index, value)
            );

        public AttributeModel SetProperty(String name, Object value)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return this.With(
                am => am._properties = am._properties.SetItem(name, value)
            );
        }

        Object IHarshCloneable.Clone() => MemberwiseClone();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeModel));
    }
}
