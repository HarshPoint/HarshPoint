using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal abstract partial class CommandBuilder
    {
        private ImmutableDictionary<String, ParameterBuilder> _parameters
            = ImmutableDictionary<String, ParameterBuilder>.Empty;

        protected CommandBuilder(HarshProvisionerMetadata metadata)
        {
            if (metadata == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(metadata));
            }

            Metadata = metadata;
            DefaultParameterSetName = metadata.DefaultParameterSet?.Name;

            foreach (var grouping in metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                if (parameters.Any(p => p.IsCommonParameter))
                {
                    parameters = parameters.Take(1);
                }

                var attributes = parameters.Select(CreateParameterAttribute);

                var synthesized = new ParameterBuilderSynthesized(
                    property.Name,
                    property.PropertyType,
                    Metadata.ObjectType,
                    attributes
                );

                SetParameter(property.Name, synthesized);
            }
        }

        public HashSet<String> Aliases { get; }
            = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

        public Collection<AttributeData> Attributes { get; }
            = new Collection<AttributeData>();

        public String DefaultParameterSetName { get; set; }

        public Boolean HasInputObject { get; set; }

        public HashSet<String> ImportedNamespaces { get; }
            = new HashSet<String>(StringComparer.Ordinal);

        public String Namespace { get; set; }

        protected IChildCommandBuilder ChildBuilder { get; set; }

        protected HarshProvisionerMetadata Metadata { get; }

        public abstract Type ProvisionerType { get; }

        internal void SetParameter(
            String name,
            ParameterBuilder parameter
        )
        {
            ValidateParameterName(name);

            var existing = _parameters.GetValueOrDefault(name);

            if (parameter == existing)
            {
                return;
            }

            _parameters = _parameters.SetItem(
                name,
                parameter.CreateFrom(existing)
            );
        }

        private static void AssignParameterPositions(
            IEnumerable<ShellployCommandProperty> properties
        )
        {
            var currentPosition = 0;

            foreach (var prop in properties.Where(p => p.IsPositional))
            {
                foreach (var attr in prop.ParameterAttributes)
                {
                    attr.NamedArguments["Position"] = currentPosition;
                }

                currentPosition++;
            }
        }

        internal static void ValidateParameterName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (ReservedNames.Contains(name))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(name),
                    SR.CommandBuilder_ReservedName,
                    name
                );
            }
        }

        private static AttributeData CreateParameterAttribute(Parameter param)
        {
            var data = new AttributeData(typeof(SMA.ParameterAttribute));

            if (param.IsMandatory)
            {
                data.NamedArguments["Mandatory"] = true;
            }

            if (!param.IsCommonParameter)
            {
                data.NamedArguments["ParameterSetName"] = param.ParameterSetName;
            }

            return data;
        }

        private static ParameterBuilder SetValueFromPipelineByPropertyName(
            ParameterBuilder parameter
        )
            => new ParameterBuilderAttributeNamedArgument(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipelineByPropertyName",
                true,
                parameter
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder));

        private static readonly ImmutableHashSet<String> ReservedNames
            = ImmutableHashSet.Create(
                StringComparer.OrdinalIgnoreCase,
                ShellployCommand.InputObjectPropertyName
            );
    }
}
