using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderSynthesized : ParameterBuilder
    {
        internal ParameterBuilderSynthesized(
            String name,
            Type type,
            Type provisionerType = null,
            params AttributeData[] attributes
        )
            : base(name)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

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

        /// <summary>
        /// <see cref="ParameterBuilderSynthesized"/> Always has to be at the 
        /// end of the chain.
        /// </summary>
        public override ParameterBuilder InsertIntoContainer(
            ParameterBuilder existing
        )
        {
            if (existing == null)
            {
                return this;
            }

            if (existing.HasElementsOfType<ParameterBuilderSynthesized>())
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.CommandParameterSynthesized_AttemptedToNest
                );
            }

            return existing.Append(this);
        }

        protected internal override ParameterBuilder Accept(ParameterBuilderVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitSynthesized(this);
        }

        private static Boolean IsParameterAttribute(AttributeData data)
            => data.AttributeType == typeof(SMA.ParameterAttribute);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<ParameterBuilderSynthesized>();
    }
}
