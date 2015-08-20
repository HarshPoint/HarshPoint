using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal class CommandBuilder<TProvisioner> : ICommandBuilder
        where TProvisioner : HarshProvisionerBase
    {
        private ImmutableDictionary<String, CommandParameter> _parameters
            = ImmutableDictionary<String, CommandParameter>.Empty;

        private IChildCommandBuilder _childBuilder;

        public CommandBuilder()
        {
            Aliases.Add(
                Regex.Replace(Metadata.ObjectType.Name, "^Harsh(Modify)?", "")
            );

            foreach (var grouping in Metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                if (parameters.Any(p => p.IsCommonParameter))
                {
                    parameters = parameters.Take(1);
                }

                var attributes = parameters
                    .Select(CreateParameterAttribute)
                    .ToArray();

                SetParameter(
                    property.Name,
                    isPositional: false,
                    parameter: new CommandParameterSynthesized(
                        property.PropertyType,
                        Metadata.ObjectType,
                        attributes
                    )
                );
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
            where TParent : HarshProvisionerBase
        {
            var result = new ChildCommandBuilder<TProvisioner, TParent>();
            _childBuilder = result;
            action(result);
        }

        public CommandParameterFactory<TProvisioner> Parameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression);

        public CommandParameterFactory<TProvisioner> Parameter(String name)
            => GetParameterFactory(name);

        public CommandParameterFactory<TProvisioner> PositionalParameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression, isPositional: true);

        public CommandParameterFactory<TProvisioner> PositionalParameter(
            String name
        )
            => GetParameterFactory(name, isPositional: true);

        public IEnumerable<ShellployCommandProperty> GetProperties(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        )
        {
            var parametersSorted =
                _parameters.Values
                .OrderBy(param => param.Position ?? Int32.MaxValue)
                .ToList();

            if (HasInputObject)
            {
                parametersSorted.Add(new CommandParameterInputObject());
            }

            var properties = GetParentProperties(builders)
                .Concat(
                    parametersSorted.SelectMany(p => p.Synthesize())
                )
                .ToArray();

            SetValueFromPipelineByPropertyName(properties);
            AssignParameterPositions(properties);

            return properties;
        }

        private static void SetValueFromPipelineByPropertyName(
            IEnumerable<ShellployCommandProperty> properties
        )
        {
            var attrs = from prop in properties
                        where !prop.IsInputObject
                        from attr in prop.ParameterAttributes
                        select attr;

            foreach (var attr in attrs)
            {
                attr.NamedArguments["ValueFromPipelineByPropertyName"] = true;
            }
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

        public ImmutableList<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        )
        {
            var parentBuilder = GetParentBuilder(builders);

            if (parentBuilder != null)
            {
                return parentBuilder
                    .GetParentProvisionerTypes(builders)
                    .Add(parentBuilder.ProvisionerType);
            }

            return ImmutableList<Type>.Empty;
        }

        public ShellployCommand ToCommand()
            => ToCommand(null);

        public ShellployCommand ToCommand(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        )
        {
            if (builders == null)
            {
                builders = ImmutableDictionary<Type, ICommandBuilder>.Empty;
            }

            var properties = GetProperties(builders);

            var verb = SMA.VerbsCommon.New;
            var noun = ProvisionerType.Name;

            return new ShellployCommand
            {
                Aliases = Aliases.ToImmutableArray(),
                ClassName = $"{verb}{noun}Command",
                ContextType = Metadata.ContextType,
                HasInputObject = properties.Any(p => p.IsInputObject),
                Name = $"{verb}-{noun}",
                Namespace = Namespace,
                Noun = noun,
                Properties = properties.ToImmutableArray(),
                ParentProvisionerTypes = GetParentProvisionerTypes(builders),
                ProvisionerType = ProvisionerType,
                Usings = ImportedNamespaces.ToImmutableArray(),
                Verb = Tuple.Create(
                    typeof(SMA.VerbsCommon),
                    nameof(SMA.VerbsCommon.New)
                ),
            };
        }

        internal void SetParameter(
            String name,
            Boolean isPositional,
            CommandParameter parameter
        )
        {
            var existing = _parameters.GetValueOrDefault(name);

            if (parameter == existing)
            {
                return;
            }

            parameter = parameter.CreateFrom(existing);
            parameter.Name = name;

            if (isPositional && !parameter.Position.HasValue)
            {
                parameter.Position = _parameters.Count;
            }

            _parameters = _parameters.SetItem(
                name,
                parameter
            );
        }

        private ICommandBuilder GetParentBuilder(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        )
        {
            if (builders == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builders));
            }

            if (_childBuilder == null)
            {
                return null;
            }

            return builders[_childBuilder.Type];
        }

        private IEnumerable<ShellployCommandProperty> GetParentProperties(
            IReadOnlyDictionary<Type, ICommandBuilder> builders
        )
        {
            if (_childBuilder != null)
            {
                var parentBuilder = GetParentBuilder(builders);

                var parentProperties = parentBuilder.GetProperties(
                    builders
                );

                return _childBuilder.Process(
                    parentProperties
                );
            }

            return Enumerable.Empty<ShellployCommandProperty>();
        }

        private CommandParameterFactory<TProvisioner> GetParameterFactory(
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

        private CommandParameterFactory<TProvisioner> GetParameterFactory(
            String name,
            Boolean isPositional = false
        )
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return new CommandParameterFactory<TProvisioner>(
                this,
                name,
                isPositional
            );
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

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder<>));

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));
    }
}