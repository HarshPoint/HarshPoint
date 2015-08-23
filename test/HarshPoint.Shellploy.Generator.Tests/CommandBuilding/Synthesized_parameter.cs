using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
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
        public void Has_Identifier()
        {
            var builder = new NewObjectCommandBuilder<EmptyProvisioner>();
            builder.Parameter("Synth").Synthesize(typeof(Int32));

            var command = builder.ToCommand();

            var prop = Assert.Single(command.Properties);
            Assert.Equal("Synth", prop.Identifier);
        }

        [Fact]
        public void Has_Type()
        {
            var builder = new NewObjectCommandBuilder<EmptyProvisioner>();
            builder.Parameter("Synth").Synthesize(typeof(Int32));

            var command = builder.ToCommand();

            var prop = Assert.Single(command.Properties);
            var synth = Assert.Single(
                prop.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(Int32), synth.PropertyType);
        }

        [Fact]
        public void Is_created_even_when_not_first_in_the_list()
        {
            var builder = new NewObjectCommandBuilder<EmptyProvisioner>();

            builder.Parameter("Synth")
                .SetDefaultValue(42)
                .Synthesize(typeof(Int32));

            var command = builder.ToCommand();

            var prop = Assert.Single(command.Properties);

            var defVal = Assert.Single(
                prop.ElementsOfType<PropertyModelDefaultValue>()
            );

            var synth = Assert.Single(
                prop.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(42, defVal.DefaultValue);
            Assert.Equal("Synth", synth.Identifier);
        }



        [Fact]
        public void Can_not_have_existing_name()
        {
            var builder = new NewObjectCommandBuilder<TestProvisioner>();

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
