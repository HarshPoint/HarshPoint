using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterSynthesized : CommandParameter
    {
        internal CommandParameterSynthesized(
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

        internal override IEnumerable<ShellployCommandProperty> Synthesize()
            => ImmutableArray.Create(
                new ShellployCommandProperty()
                {
                    Identifier = Name,
                    PropertyName = Name,
                    ProvisionerType = ProvisionerType,
                    Type = ParameterType,
                    Attributes = Attributes,
                    IsPositional = SortOrder.HasValue
                }
            );

        internal override CommandParameter CreateFrom(CommandParameter previous)
        {
            if (previous == null)
            {
                return this;
            }

            AppendThisTo(previous);
            SortOrder = previous.SortOrder;

            return previous;
        }

        private void AppendThisTo(CommandParameter appendTo)
        {
            if (appendTo is CommandParameterSynthesized)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.CommandParameterSynthesized_AttemptedToNest
                );
            }

            while (appendTo.Previous != null)
            {
                if (appendTo is CommandParameterSynthesized)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.CommandParameterSynthesized_AttemptedToNest
                    );
                }

                appendTo = appendTo.Previous;
            }

            appendTo.Previous = this;
        }

        private static Boolean IsParameterAttribute(AttributeData data)
            => data.AttributeType == typeof(SMA.ParameterAttribute);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<CommandParameterSynthesized>();
    }
}
