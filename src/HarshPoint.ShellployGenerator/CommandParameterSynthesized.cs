using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal class CommandParameterSynthesized : CommandParameter
    {
        internal CommandParameterSynthesized(
            Type type,
            IEnumerable<AttributeData> attributes
        )
            : this(type, null, attributes)
        {
        }

        internal CommandParameterSynthesized(
            Type type,
            Type provisionerType,
            IEnumerable<AttributeData> attributes
        )
        {
            ParameterType = type;
            ProvisionerType = provisionerType;
            Attributes = attributes.ToImmutableArray();

            if (!Attributes.Any(IsParameterAttribute))
            {
                Attributes = ImmutableArray.Create(
                    new AttributeData(typeof(SMA.ParameterAttribute))
                );
            }
        }

        public IReadOnlyList<AttributeData> Attributes { get; }
            = new Collection<AttributeData>();

        public Type ParameterType { get; }

        public Type ProvisionerType { get; }

        internal override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray.Create(
                new ShellployCommandProperty()
                {
                    Identifier = Name,
                    PropertyName = Name,
                    ProvisionerType = ProvisionerType,
                    Type = ParameterType,
                    Attributes = Attributes,
                    IsPositional = Position.HasValue
                }
            );

        internal override CommandParameter CreateFrom(CommandParameter previous)
        {
            if (previous == null)
            {
                return this;
            }

            AppendThisTo(previous);

            Name = previous.Name;
            Position = previous.Position;

            return previous;
        }

        private void AppendThisTo(CommandParameter parameter)
        {
            var last = parameter;

            while (last.Previous != null)
            {
                if (last is CommandParameterSynthesized)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.CommandParameterSynthesized_AttemptedToNest
                    );
                }

                last = last.Previous;
            }

            last.Previous = this;
        }

        private static Boolean IsParameterAttribute(AttributeData data)
            => data.AttributeType == typeof(SMA.ParameterAttribute);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<CommandParameterSynthesized>();
    }
}
