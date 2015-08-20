using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Synthesized_parameter : SeriloggedTest
    {
        public Synthesized_parameter(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Is_created_even_when_not_first_in_the_list()
        {
            var builder = new CommandBuilder<EmptyProvisioner>();

            builder.Parameter("Synth")
                .SetDefaultValue(42)
                .Synthesize(typeof(Int32));

            var command = builder.ToCommand();

            var prop = Assert.Single(command.Properties);
            Assert.Equal(42, prop.DefaultValue);
            Assert.Equal("Synth", prop.PropertyName);
        }

        [Fact]
        public void Can_not_have_reserved_name_InputObject()
        {
            var builder = new CommandBuilder<EmptyProvisioner>();

            Assert.Throws<ArgumentException>(() =>
                builder.Parameter("InputObject").Synthesize(typeof(Int32))
            );
        }


        [Fact]
        public void Can_not_have_existing_name()
        {
            var builder = new CommandBuilder<TestProvisioner>();

            Assert.Throws<InvalidOperationException>(() =>
                builder.Parameter("Param").Synthesize(typeof(Int32))
            );
        }

        private sealed class EmptyProvisioner : HarshProvisioner
        {
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String Param { get; set; }
        }
    }
}
