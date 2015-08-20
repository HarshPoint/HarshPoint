using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal partial class CommandBuilder<TProvisioner>
    {
        private IChildCommandBuilder _childBuilder;

        private Int32 _nextPositionalParam;

        private ImmutableDictionary<String, ParameterBuilder> _parameters
            = ImmutableDictionary<String, ParameterBuilder>.Empty;

        public CommandBuilder()
        {
            foreach (var grouping in Metadata.PropertyParameters)
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

        public Boolean HasInputObject { get; set; }

        public HashSet<String> ImportedNamespaces { get; }
            = new HashSet<String>(StringComparer.Ordinal);

        public String Namespace { get; set; }

        public Type ProvisionerType => typeof(TProvisioner);

        public void AsChildOf<TParent>(
            Action<ChildCommandBuilder<TProvisioner, TParent>> action
        )
        {
            var result = new ChildCommandBuilder<TProvisioner, TParent>();
            _childBuilder = result;
            action(result);
        }

        public ParameterBuilderFactory<TProvisioner> Parameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression);

        public ParameterBuilderFactory<TProvisioner> Parameter(String name)
            => GetParameterFactory(name);

        public ParameterBuilderFactory<TProvisioner> PositionalParameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression, isPositional: true);

        public ParameterBuilderFactory<TProvisioner> PositionalParameter(
            String name
        )
            => GetParameterFactory(name, isPositional: true);

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

        internal void ValidateParameterName(String name)
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

        private ParameterBuilderFactory<TProvisioner> GetParameterFactory(
            Expression<Func<TProvisioner, Object>> expression,
            Boolean isPositional = false
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var name = expression.ExtractLastPropertyAccess().Name;
            return GetParameterFactory(name, isPositional);
        }

        private ParameterBuilderFactory<TProvisioner> GetParameterFactory(
            String name,
            Boolean isPositional = false
        )
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (isPositional)
            {
                SetParameter(
                    name,
                    new ParameterBuilderPositional(_nextPositionalParam)
                );

                _nextPositionalParam++;
            }

            return new ParameterBuilderFactory<TProvisioner>(
                this,
                name
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
            = HarshLog.ForContext(typeof(CommandBuilder<>));

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));

        private static readonly ImmutableHashSet<String> ReservedNames
            = ImmutableHashSet.Create(
                StringComparer.OrdinalIgnoreCase,
                ShellployCommand.InputObjectPropertyName
            );
    }
}