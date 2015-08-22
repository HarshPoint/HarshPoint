using HarshPoint.Provisioning;
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
        private readonly CommandBuilder _builder;
        private readonly CommandModel _command;

        public Without_configuration(ITestOutputHelper output) : base(output)
        {
            _builder = new NewObjectCommandBuilder<EmptyProvisioner>();
            _command = _builder.ToCommand();
        }

        [Fact]
        public void Has_ClassName()
        {
            Assert.Equal($"New{nameof(EmptyProvisioner)}Command", _builder.ClassName);
        }

        [Fact]
        public void Doesnt_have_InputObject()
        {
            Assert.False(_builder.HasInputObject);
        }

        [Fact]
        public void Has_name()
        {
            Assert.Equal($"New-{nameof(EmptyProvisioner)}", _builder.Name);
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
        public void Has_noun_verb()
        {
            var attr = Assert.Single(
                _command.Attributes,
                a => a.AttributeType == typeof(SMA.CmdletAttribute)
            );
            Assert.Equal(SMA.VerbsCommon.New, attr.Arguments[0]);
            Assert.Equal(nameof(EmptyProvisioner), attr.Arguments[1]);
        }

        [Fact]
        public void Has_output_type()
        {
            var attr = Assert.Single(
                _command.Attributes,
                a => a.AttributeType == typeof(SMA.OutputTypeAttribute)
            );
            Assert.Equal(typeof(EmptyProvisioner), attr.Arguments[0]);
        }

        [Fact]
        public void Doesnt_have_DefaultParameterSet()
        {
            var attr = Assert.Single(
                _command.Attributes,
                a => a.AttributeType == typeof(SMA.CmdletAttribute)
            );

            Assert.DoesNotContain(
                attr.Properties,
                a => a.Key == "DefaultParameterSetName"
            );
        }

        private class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
