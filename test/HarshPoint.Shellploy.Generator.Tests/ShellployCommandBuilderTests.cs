using HarshPoint.Provisioning;
using HarshPoint.Tests;
using Moq;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.ShellployGenerator.Tests
{
    public class ShellployCommandBuilderTests : SeriloggedTest
    {
        public class HarshEmptyTestProvisioner : HarshProvisioner
        {
        }

        public class HarshCustomChildrenTestProvisioner : HarshProvisioner
        {
            [Parameter()]
            public new String Children { get; set; }
        }

        public class HarshTestProvisioner : HarshProvisioner
        {
            [Parameter()]
            public String BasicParam { get; set; }

            [Parameter(Mandatory = true)]
            public String MandatoryParam { get; set; }

            [Parameter(ParameterSetName = "A")]
            public String ParamSetA { get; set; }

            [Parameter(ParameterSetName = "B")]
            public String ParamSetB { get; set; }

            [Parameter(ParameterSetName = "A")]
            [Parameter(ParameterSetName = "B", Mandatory = true)]
            public String ParamSetA_BMandatory { get; set; }
        }

        public ShellployCommandBuilderTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void Creates_command_without_configuration()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshEmptyTestProvisioner>()
                .InNamespace(ns)
                .ToCommand();

            Assert.Equal("New" + nameof(HarshEmptyTestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshEmptyTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshEmptyTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);
            Assert.Empty(command.Properties);
        }

        [Fact]
        public void Creates_command_with_children()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshEmptyTestProvisioner>()
                .InNamespace(ns)
                .HasChildren()
                .ToCommand();

            Assert.Equal("New" + nameof(HarshEmptyTestProvisioner) + "Command", command.ClassName);
            Assert.True(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshEmptyTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshEmptyTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.Equal(1, command.Properties.Count);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Name);

            var propName = ShellployCommand.ChildrenPropertyName;
            Assert.Equal(typeof(ScriptBlock), properties[propName].Type);
            Assert.Null(properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(0, properties[propName].ParameterAttributes[0].Position);
            Assert.True(properties[propName].ParameterAttributes[0].ValueFromPipeline);
        }

        [Fact]
        public void Creates_command_with_parameters()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshTestProvisioner>()
                .InNamespace(ns)
                .ToCommand();

            Assert.Equal("New" + nameof(HarshTestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Name);

            var propName = nameof(HarshTestProvisioner.BasicParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[0].ValueFromPipeline);

            propName = nameof(HarshTestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.True(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[0].ValueFromPipeline);

            propName = nameof(HarshTestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("A", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[0].ValueFromPipeline);

            propName = nameof(HarshTestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("B", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[0].ValueFromPipeline);

            propName = nameof(HarshTestProvisioner.ParamSetA_BMandatory);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(2, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("A", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[0].ValueFromPipeline);
            Assert.True(properties[propName].ParameterAttributes[1].Mandatory);
            Assert.Equal("B", properties[propName].ParameterAttributes[1].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[1].Position);
            Assert.False(properties[propName].ParameterAttributes[1].ValueFromPipeline);
        }

        [Fact]
        public void Creates_command_with_positional_parameters()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshTestProvisioner>()
                .InNamespace(ns)
                .AddPositionalParameter(x => x.BasicParam)
                .AddPositionalParameter<String>("CustomParameter1")
                .AddPositionalParameter<Int32>("CustomParameter2",
                    new ShellployCommandPropertyParameterAttribute()
                    {
                        Mandatory = true,
                        Position = 50,
                        ParameterSet = "asdf",
                    },
                    new ShellployCommandPropertyParameterAttribute()
                    {
                        Position = 51,
                        ParameterSet = "qwer",
                    }
                )
                .AddPositionalParameter(x => x.ParamSetB)
                .HasChildren()
                .ToCommand();

            Assert.Equal("New" + nameof(HarshTestProvisioner) + "Command", command.ClassName);
            Assert.True(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Name);

            var propName = nameof(HarshTestProvisioner.BasicParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(0, properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.True(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("A", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("B", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(3, properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetA_BMandatory);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(2, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("A", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
            Assert.True(properties[propName].ParameterAttributes[1].Mandatory);
            Assert.Equal("B", properties[propName].ParameterAttributes[1].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[1].Position);

            propName = "CustomParameter1";
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(1, properties[propName].ParameterAttributes[0].Position);

            propName = "CustomParameter2";
            Assert.Equal(typeof(Int32), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(2, properties[propName].ParameterAttributes.Count);
            Assert.True(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("asdf", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(2, properties[propName].ParameterAttributes[0].Position);
            Assert.False(properties[propName].ParameterAttributes[1].Mandatory);
            Assert.Equal("qwer", properties[propName].ParameterAttributes[1].ParameterSet);
            Assert.Equal(2, properties[propName].ParameterAttributes[1].Position);

            propName = ShellployCommand.ChildrenPropertyName;
            Assert.Equal(typeof(ScriptBlock), properties[propName].Type);
            Assert.Null(properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Equal(4, properties[propName].ParameterAttributes[0].Position);
        }

        [Fact]
        public void Creates_command_with_custom_children()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshCustomChildrenTestProvisioner>()
                .InNamespace(ns)
                .ToCommand();

            Assert.Equal("New" + nameof(HarshCustomChildrenTestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshCustomChildrenTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshCustomChildrenTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Name);

            var propName = nameof(HarshCustomChildrenTestProvisioner.Children);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshCustomChildrenTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);
        }

        [Fact]
        public void Command_with_custom_children_and_haschildren_fails()
        {
            var ns = "MyNamespace";
            var commandBuilder = new ShellployCommandBuilder<HarshCustomChildrenTestProvisioner>()
                .InNamespace(ns)
                .HasChildren();

            Assert.Throws<InvalidOperationException>(() => commandBuilder.ToCommand());
        }

        [Fact]
        public void Creates_command_with_ignored_parameters()
        {
            var ns = "MyNamespace";
            var command = new ShellployCommandBuilder<HarshTestProvisioner>()
                .InNamespace(ns)
                .IgnoreParameter(x => x.BasicParam)
                .IgnoreParameter(x => x.ParamSetA_BMandatory)
                .ToCommand();

            Assert.Equal("New" + nameof(HarshTestProvisioner) + "Command", command.ClassName);
            Assert.False(command.HasChildren);
            Assert.Empty(command.ParentProvisionerTypes);
            Assert.Equal(nameof(HarshTestProvisioner), command.Noun);
            Assert.Equal(typeof(HarshTestProvisioner), command.ProvisionerType);
            Assert.Equal(typeof(VerbsCommon), command.Verb.Item1);
            Assert.Equal(nameof(VerbsCommon.New), command.Verb.Item2);
            Assert.Equal(ns, command.Namespace);

            Assert.NotEmpty(command.Properties);
            var properties = command.Properties.ToImmutableDictionary(prop => prop.Name);

            var propName = nameof(HarshTestProvisioner.BasicParam);
            Assert.DoesNotContain(propName, properties.Keys);

            propName = nameof(HarshTestProvisioner.MandatoryParam);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.True(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Null(properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetA);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("A", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetB);
            Assert.Equal(typeof(String), properties[propName].Type);
            Assert.Equal(typeof(HarshTestProvisioner), properties[propName].AssignmentOnType);
            Assert.Equal(1, properties[propName].ParameterAttributes.Count);
            Assert.False(properties[propName].ParameterAttributes[0].Mandatory);
            Assert.Equal("B", properties[propName].ParameterAttributes[0].ParameterSet);
            Assert.Null(properties[propName].ParameterAttributes[0].Position);

            propName = nameof(HarshTestProvisioner.ParamSetA_BMandatory);
            Assert.DoesNotContain(propName, properties.Keys);
        }
    }
}
