using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
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
        public void Has_verb_noun_name()
        {
            Assert.Equal(typeof(SMA.VerbsCommon), _command.Verb.Item1);
            Assert.Equal(nameof(SMA.VerbsCommon.New), _command.Verb.Item2);
            Assert.Equal(nameof(EmptyProvisioner), _command.Noun);
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

        private class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
