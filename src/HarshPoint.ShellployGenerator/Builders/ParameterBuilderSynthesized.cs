using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderSynthesized : ParameterBuilder
    {
        internal ParameterBuilderSynthesized(
            String name,
            Type type,
            Type provisionerType = null,
            IEnumerable<AttributeData> attributes = null
        )
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            Name = name;
            ParameterType = type;
            ProvisionerType = provisionerType;

            Attributes = attributes?.ToImmutableArray() ??
                ImmutableArray<AttributeData>.Empty;

            if (!Attributes.Any(IsParameterAttribute))
            {
                Attributes = Attributes.Add(
                    new AttributeData(typeof(SMA.ParameterAttribute))
                );
            }
        }

        public ImmutableArray<AttributeData> Attributes { get; }

        public String Name { get; }

        public Type ParameterType { get; }

        public Type ProvisionerType { get; }

        public override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray.Create(
                new ShellployCommandProperty()
                {
                    Identifier = Name,
                    PropertyName = Name,
                    ProvisionerType = ProvisionerType,
                    Type = ParameterType,
                    Attributes = Attributes,
                }
            );

        public override ParameterBuilder WithNext(ParameterBuilder next)
        {
            if (next == null)
            {
                return this;
            }

            if (next.HasElementOfType<ParameterBuilderSynthesized>())
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.CommandParameterSynthesized_AttemptedToNest
                );
            }

            return next.Append(WithSortOrder(next.SortOrder));
        }

        private static Boolean IsParameterAttribute(AttributeData data)
            => data.AttributeType == typeof(SMA.ParameterAttribute);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ParameterBuilderSynthesized>();
    }
}
