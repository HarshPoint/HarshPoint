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
        private readonly AttributeData _cmdletAttribute
            = new AttributeData(typeof(SMA.CmdletAttribute))
            {
                ConstructorArguments = { null, null }
            };

        private CommandBuilderContext _context = EmptyContext;

        protected CommandBuilder(HarshProvisionerMetadata metadata)
        {
            if (metadata == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(metadata));
            }

            Attributes.Add(_cmdletAttribute);

            Attributes.Add(new AttributeData(typeof(SMA.OutputTypeAttribute))
            {
                ConstructorArguments = { metadata.ObjectType }
            });

            DefaultParameterSetName = metadata.DefaultParameterSet?.Name;
            Noun = metadata.ObjectType.Name;
            Verb = SMA.VerbsCommon.New;

            foreach (var grouping in metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                ValidateParameterName(property.Name);

                if (parameters.Any(p => p.IsCommonParameter))
                {
                    parameters = parameters.Take(1);
                }

                var attributes = parameters.Select(CreateParameterAttribute);

                var synthesized = new ParameterBuilderSynthesized(
                    property.Name,
                    property.PropertyType,
                    metadata.ObjectType,
                    attributes
                );

                Parameters.Update(property.Name, synthesized);
            }
        }

        public HashSet<String> Aliases { get; }
            = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

        public Collection<AttributeData> Attributes { get; }
            = new Collection<AttributeData>();

        public String DefaultParameterSetName
        {
            get
            {
                return (String)_cmdletAttribute.NamedArguments
                    .GetValueOrDefault("DefaultParameterSetName");
            }
            set
            {
                _cmdletAttribute.NamedArguments["DefaultParameterSetName"]
                    = value;
            }
        }

        public Boolean HasInputObject { get; set; }

        public HashSet<String> ImportedNamespaces { get; }
            = new HashSet<String>(StringComparer.Ordinal);

        public String Namespace { get; set; }

        public String Noun
        {
            get { return (String)_cmdletAttribute.ConstructorArguments[1]; }
            set { _cmdletAttribute.ConstructorArguments[1] = value; }
        }

        public abstract Type ProvisionerType { get; }

        public String Verb
        {
            get { return (String)_cmdletAttribute.ConstructorArguments[0]; }
            set { _cmdletAttribute.ConstructorArguments[0] = value; }
        }

        protected IChildCommandBuilder ChildBuilder { get; set; }

        protected internal CommandBuilderContext Context
        {
            get
            {
                if (_context == null)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.CommandBuilder_NoContext
                    );
                }

                return _context;
            }
            internal set { _context = value; }
        }

        protected abstract ParameterBuilderContainer Parameters
        {
            get;
        }

        protected CommandBuilder ParentBuilder
        {
            get
            {
                if (ChildBuilder != null)
                {

                    return Context.GetBuilder(ChildBuilder.ProvisionerType);
                }

                return null;
            }
        }

        internal IImmutableList<Type> ParentProvisionerTypes
        {
            get
            {
                if (ParentBuilder != null)
                {
                    return ParentBuilder
                        .ParentProvisionerTypes
                        .Add(ParentBuilder.ProvisionerType);
                }

                return ImmutableList<Type>.Empty;
            }
        }

        internal ShellployCommand ToCommand()
        {
            var properties = GetParametersRecursively()
                .SelectMany(p => p.Value.Synthesize())
                .ToImmutableArray();

            AssignParameterPositions(properties);

            return new ShellployCommand
            {
                Attributes = Attributes.ToImmutableArray(),
                Aliases = Aliases.ToImmutableArray(),
                ClassName = $"{Verb}{Noun}Command",
                HasInputObject = properties.Any(p => p.IsInputObject),
                Name = $"{Verb}-{Noun}",
                Namespace = Namespace,
                Properties = properties,
                ParentProvisionerTypes = ParentProvisionerTypes,
                ProvisionerType = ProvisionerType,
                Usings = ImportedNamespaces.ToImmutableArray(),
            };
        }

        internal ImmutableList<KeyValuePair<String, ParameterBuilder>> 
        GetParametersRecursively()
        {
            var parametersSorted
                = ImmutableList<KeyValuePair<String, ParameterBuilder>>.Empty;

            if (ChildBuilder != null)
            {
                parametersSorted = ChildBuilder.Parameters
                    .ApplyTo(ParentBuilder.GetParametersRecursively())
                    .ToImmutableList();
            }

            parametersSorted = parametersSorted.AddRange(
                from param in Parameters
                select param.WithValue(
                    SetValueFromPipelineByPropertyName(param.Value)
                )
            );

            if (HasInputObject)
            {
                parametersSorted = parametersSorted.Add(InputObjectParameter);
            }

            return parametersSorted;
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

        private static readonly KeyValuePair<String, ParameterBuilder> InputObjectParameter
            = new KeyValuePair<String, ParameterBuilder>(
                ShellployCommand.InputObjectPropertyName,
                new ParameterBuilderInputObject(
                    new ParameterBuilderAttributeNamedArgument(
                        typeof(SMA.ParameterAttribute),
                        "ValueFromPipeline",
                        true,
                        new ParameterBuilderSynthesized(
                            ShellployCommand.InputObjectPropertyName,
                            typeof(Object)
                        )
                    )
                )
            );

        private static readonly CommandBuilderContext EmptyContext
            = new CommandBuilderContext();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder));

        private static readonly ImmutableHashSet<String> ReservedNames
            = ImmutableHashSet.Create(
                StringComparer.OrdinalIgnoreCase,
                ShellployCommand.InputObjectPropertyName
            );
    }
}
