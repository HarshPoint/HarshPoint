using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    partial class CommandBuilder<TProvisioner> : ICommandBuilder
    {
        IEnumerable<ShellployCommandProperty> ICommandBuilder.GetProperties(
            CommandBuilderContext context
        )
        {
            var parametersSorted =
                _parameters.Values
                .OrderBy(param => param.SortOrder ?? Int32.MaxValue)
                .Select(SetValueFromPipelineByPropertyName)
                .ToList();

            if (HasInputObject)
            {
                parametersSorted.Add(CreateInputObjectParameter());
            }

            var parentProperties = GetParentProperties(context);

            var myProperties = parametersSorted
                .SelectMany(p => p.Synthesize());

            var allProperties = parentProperties
                .Concat(myProperties)
                .ToArray();

            AssignParameterPositions(allProperties);

            return allProperties;
        }

        private static ParameterBuilder CreateInputObjectParameter()
        {
            var inputObject = new ParameterBuilderInputObject(
                    new ParameterBuilderSynthesized(
                    ParameterBuilderInputObject.Name,
                    typeof(Object)
                )
            );

            return new ParameterBuilderAttributeNamedArgument(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipeline",
                true,
                inputObject
            );
        }

        ImmutableList<Type> ICommandBuilder.GetParentProvisionerTypes(
            CommandBuilderContext context
        )
        {
            var parentBuilder = GetParentBuilder(context);

            if (parentBuilder != null)
            {
                return parentBuilder
                    .GetParentProvisionerTypes(context)
                    .Add(parentBuilder.ProvisionerType);
            }

            return ImmutableList<Type>.Empty;
        }

        ShellployCommand ICommandBuilder.ToCommand(
            CommandBuilderContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var self = (ICommandBuilder)this;
            var properties = self.GetProperties(context);

            var verb = SMA.VerbsCommon.New;
            var noun = ProvisionerType.Name;

            if (!Attributes.OfType<CmdletAttribute>().Any())
            {
                Attributes.Add(CreateCmdletAttributeData(verb, noun));
            }

            if (!Attributes.OfType<OutputTypeAttribute>().Any())
            {
                Attributes.Add(CreateOutputTypeAttributeData());
            }

            return new ShellployCommand
            {
                Attributes = Attributes.ToImmutableList(),
                Aliases = Aliases.ToImmutableArray(),
                ClassName = $"{verb}{noun}Command",
                ContextType = Metadata.ContextType,
                HasInputObject = properties.Any(p => p.IsInputObject),
                Name = $"{verb}-{noun}",
                Namespace = Namespace,
                Properties = properties.ToImmutableArray(),
                ParentProvisionerTypes = self.GetParentProvisionerTypes(context),
                ProvisionerType = ProvisionerType,
                Usings = ImportedNamespaces.ToImmutableArray(),
            };
        }

        private AttributeData CreateCmdletAttributeData(
            String verb,
            String noun
        )
            => new AttributeData(typeof(CmdletAttribute))
            {
                ConstructorArguments =
                {
                    verb,
                    noun,
                },
                NamedArguments =
                {
                    ["DefaultParameterSetName"] = DefaultParameterSetName,
                },
            };

        private AttributeData CreateOutputTypeAttributeData()
            => new AttributeData(typeof(OutputTypeAttribute))
            {
                ConstructorArguments =
                {
                    ProvisionerType,
                }
            };

        private ICommandBuilder GetParentBuilder(CommandBuilderContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (_childBuilder == null)
            {
                return null;
            }

            return context.GetBuilder(_childBuilder.ProvisionerType);
        }

        private IEnumerable<ShellployCommandProperty> GetParentProperties(
            CommandBuilderContext context
        )
        {
            if (_childBuilder != null)
            {
                var parentBuilder = GetParentBuilder(context);

                var parentProperties = parentBuilder.GetProperties(
                    context
                );

                return _childBuilder.Process(
                    parentProperties
                );
            }

            return Enumerable.Empty<ShellployCommandProperty>();
        }
    }
}