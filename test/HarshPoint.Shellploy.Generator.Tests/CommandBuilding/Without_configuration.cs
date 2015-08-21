using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class Without_configuration : SeriloggedTest
    {
        private readonly ShellployCommand _command;

        public Without_configuration(ITestOutputHelper output) : base(output)
        {
            _command = new CommandBuilder<EmptyProvisioner>().ToCommand();
        }

        [Fact]
        public void Has_ClassName()
        {
            Assert.Equal($"New{nameof(EmptyProvisioner)}Command", _command.ClassName);
        }

        [Fact]
        public void Doesnt_have_InputObject()
        {
            Assert.False(_command.HasInputObject);
        }

        [Fact]
        public void Has_no_parents()
        {
            Assert.Empty(_command.ParentProvisionerTypes);
        }

        [Fact]
        public void Has_name()
        {
            Assert.Equal($"New-{nameof(EmptyProvisioner)}", _command.Name);
        }

        [Fact]
        public void Has_ProvisionerType()
        {
            Assert.Equal(typeof(EmptyProvisioner), _command.ProvisionerType);
        }

        [Fact]
        public void Has_no_Properties()
        {
            Assert.Empty(_command.Properties);
        }

        [Fact]
        public void Has_no_Namespace()
        {
            Assert.Null(_command.Namespace);
        }

        [Fact]
        public void Has_CmdletAttribute()
        {
            Assert.Single(
                _command.Attributes
                    .Where(attr =>
                        attr.AttributeType == typeof(SMA.CmdletAttribute))
            );
        }

        [Fact]
        public void Has_OutputTypeAttribute()
        {
            Assert.Single(
                _command.Attributes
                    .Where(attr =>
                        attr.AttributeType == typeof(SMA.OutputTypeAttribute))
            );
        }

        private Boolean WithAttributeType<T>(AttributeData attr)
            => attr.AttributeType == typeof(T);

        [Fact]
        public void Has_noun_verb()
        {
            var attr = Assert.Single(
                _command.Attributes,
                WithAttributeType<SMA.CmdletAttribute>
            );
            Assert.Equal(SMA.VerbsCommon.New, attr.ConstructorArguments[0]);
            Assert.Equal(nameof(EmptyProvisioner), attr.ConstructorArguments[1]);
        }

        [Fact]
        public void Has_output_type()
        {
            var attr = Assert.Single(
                _command.Attributes,
                WithAttributeType<SMA.OutputTypeAttribute>
            );
            Assert.Equal(typeof(EmptyProvisioner), attr.ConstructorArguments[0]);
        }

        private class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
