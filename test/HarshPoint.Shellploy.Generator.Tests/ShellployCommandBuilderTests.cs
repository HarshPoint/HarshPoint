using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.Tests;
using System;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class ShellployCommandBuilderTests : SeriloggedTest
    {
        public class HarshEmptyTestProvisioner : HarshProvisioner
        {
        }


        public ShellployCommandBuilderTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void Creates_command_with_InputObject()
        {
            var ns = "MyNamespace";
            var builder = new CommandBuilder<HarshEmptyTestProvisioner>();
            builder.Namespace = (ns);
            builder.HasInputObject = true;

            var command = builder.ToCommand();

            Assert.Equal("New" + nameof(HarshEmptyTestProvisioner) + "Command", command.ClassName);
            Assert.True(command.HasInputObject);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshEmptyTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshEmptyTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(SMA.VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(SMA.VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.Equal(1, command.Properties.Count);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Identifier);

            var propName = ShellployCommand.InputObjectPropertyName;
            Assert.Equal(typeof(Object), properties[propName].Type);
            Assert.Null(properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("__AllParameterSets", properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(0, properties[propName].Attributes[0].Position());
            Assert.True(properties[propName].Attributes[0].ValueFromPipeline());
        }

        [Fact]
        public void Creates_command_with_parameters()
        {
            var ns = "MyNamespace";
            var builder = new CommandBuilder<TestProvisioner>()
            {
                Namespace = ns
            };

            var command = builder.ToCommand();

            Assert.Equal("New" + nameof(TestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasInputObject);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(TestProvisioner), command.Noun);
            Assert.Equal(typeof(TestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(SMA.VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(SMA.VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Identifier);

            var propName = nameof(TestProvisioner.BasicParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[0].ValueFromPipeline());

            propName = nameof(TestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.True(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[0].ValueFromPipeline());

            propName = nameof(TestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("A", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[0].ValueFromPipeline());

            propName = nameof(TestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("B", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[0].ValueFromPipeline());

            propName = nameof(TestProvisioner.ParamSetA_BMandatory);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(2, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("A", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[0].ValueFromPipeline());
            Assert.True(properties[propName].Attributes[1].Mandatory());
            Assert.Equal("B", properties[propName].Attributes[1].ParameterSetName());
            Assert.Null(properties[propName].Attributes[1].Position());
            Assert.False(properties[propName].Attributes[1].ValueFromPipeline());
        }

        [Fact]
        public void Creates_command_with_positional_parameters()
        {
            var ns = "MyNamespace";
            var commandBuilder = new CommandBuilder<TestProvisioner>();
            commandBuilder.Namespace = (ns);
            commandBuilder.PositionalParameter(x => x.BasicParam);

            commandBuilder
                .PositionalParameter("CustomParameter1")
                .Synthesize(typeof(String));

            commandBuilder
                .PositionalParameter("CustomParameter2")
                .Synthesize(
                    typeof(Int32),
                    new AttributeData(typeof(SMA.ParameterAttribute))
                    {
                        NamedArguments =
                        {
                            ["Mandatory"] = true ,
                            ["Position"] = 50,
                            ["ParameterSetName"] = "asdf",
                        }
                    },

                    new AttributeData(typeof(SMA.ParameterAttribute))
                    {
                        NamedArguments =
                        {
                            ["Position"] = 51,
                            ["ParameterSetName"] = "qwer",
                        }
                    }
                );

            commandBuilder.PositionalParameter(x => x.ParamSetB);
            commandBuilder.HasInputObject = true;
            var command = commandBuilder.ToCommand();

            Assert.Equal("New" + nameof(TestProvisioner) + "Command", command.ClassName);
            Assert.True(command.HasInputObject);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(TestProvisioner), command.Noun);
            Assert.Equal(typeof(TestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(SMA.VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(SMA.VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Identifier);

            var propName = nameof(TestProvisioner.BasicParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(0, properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.True(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("A", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("B", properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(3, properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetA_BMandatory);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(2, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("A", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());
            Assert.True(properties[propName].Attributes[1].Mandatory());
            Assert.Equal("B", properties[propName].Attributes[1].ParameterSetName());
            Assert.Null(properties[propName].Attributes[1].Position());

            propName = "CustomParameter1";
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(1, properties[propName].Attributes[0].Position());

            propName = "CustomParameter2";
            Assert.Equal(typeof(Int32), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(2, properties[propName].Attributes.Count);
            Assert.True(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("asdf", properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(2, properties[propName].Attributes[0].Position());
            Assert.False(properties[propName].Attributes[1].Mandatory());
            Assert.Equal("qwer", properties[propName].Attributes[1].ParameterSetName());
            Assert.Equal(2, properties[propName].Attributes[1].Position());

            propName = ShellployCommand.InputObjectPropertyName;
            Assert.Equal(typeof(Object), properties[propName].Type);
            Assert.Null(properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Equal(4, properties[propName].Attributes[0].Position());
        }

        [Fact]
        public void Creates_command_with_ignored_parameters()
        {
            var ns = "MyNamespace";
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Namespace = ns;

            builder.Parameter(x => x.BasicParam).Ignore();
            builder.Parameter(x => x.ParamSetA_BMandatory).Ignore();
            var command = builder.ToCommand();

            Assert.Equal("New" + nameof(TestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasInputObject);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(TestProvisioner), command.Noun);
            Assert.Equal(typeof(TestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(SMA.VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(SMA.VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Identifier);

            var propName = nameof(TestProvisioner.BasicParam);
            Assert.DoesNotContain(propName, properties.Keys);

            propName = nameof(TestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.True(properties[propName].Attributes[0].Mandatory());
            Assert.Null(properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("A", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(TestProvisioner), properties[propName].ProvisionerType);
            Assert.Equal(1, properties[propName].Attributes.Count);
            Assert.False(properties[propName].Attributes[0].Mandatory());
            Assert.Equal("B", properties[propName].Attributes[0].ParameterSetName());
            Assert.Null(properties[propName].Attributes[0].Position());

            propName = nameof(TestProvisioner.ParamSetA_BMandatory);
            Assert.DoesNotContain(propName, properties.Keys);
        }
    }
}
