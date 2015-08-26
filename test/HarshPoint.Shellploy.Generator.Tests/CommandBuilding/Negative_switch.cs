using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class Negative_switch : SeriloggedTest
    {
        public Negative_switch(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Bool_is_changed_into_switch()
        {
            var builder = new NewProvisionerCommandBuilder<NonNullableTest>();
            var command = builder.ToCommand();

            var something = Assert.Single(
                command.Properties,
                p => p.Identifier == "Something"
            );

            var synth = Assert.Single(
                something.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(SMA.SwitchParameter), synth.PropertyType);
        }

        [Fact]
        public void Bool_has_no_negative_switch()
        {
            var builder = new NewProvisionerCommandBuilder<NonNullableTest>();
            var command = builder.ToCommand();

            Assert.DoesNotContain(
                command.Properties,
                p => p.Identifier == "NoSomething"
            );
        }

        [Fact]
        public void Nullable_Bool_is_changed_into_switch()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            var command = builder.ToCommand();

            var something = Assert.Single(
                command.Properties,
                p => p.Identifier == "Something"
            );

            var synth = Assert.Single(
                something.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(SMA.SwitchParameter), synth.PropertyType);
        }

        [Fact]
        public void Nullable_Bool_has_negative_switch()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            var command = builder.ToCommand();

            var noSomething = Assert.Single(
                command.Properties,
                p => p.Identifier == "NoSomething"
            );

            var synth = Assert.Single(
                noSomething.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(SMA.SwitchParameter), synth.PropertyType);
        }

        [Fact]
        public void NoNegative_has_no_negative()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            builder.Parameter(x => x.Something).NoNegative();
            var command = builder.ToCommand();

            Assert.DoesNotContain(
                command.Properties,
                p => p.Identifier == "NoSomething"
            );
        }

        [Fact]
        public void Negative_uses_renamed_name()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            builder.Parameter(x => x.Something).Rename("Different");
            var command = builder.ToCommand();

            var noSomething = Assert.Single(
                command.Properties,
                p => p.Identifier == "NoDifferent"
            );
        }

        [Fact]
        public void Negative_gets_parameter_attributes()
        {
            var builder = new NewProvisionerCommandBuilder<WithParamSets>();
            var command = builder.ToCommand();

            var negative = Assert.Single(
                command.Properties,
                p => p.Identifier == "NoParam"
            );

            var synth = Assert.Single(
                negative.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Single(
                synth.Attributes,
                a => "A".Equals(a.Properties["ParameterSetName"])
            );

            Assert.Single(
                synth.Attributes,
                a => "B".Equals(a.Properties["ParameterSetName"])
            );

            Assert.Single(
                synth.Attributes,
                a => "C".Equals(a.Properties["ParameterSetName"])
            );
        }

        [Fact]
        public void Ignored_param_doesnt_get_negative()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            builder.Parameter(x => x.Something).Ignore();
            var command = builder.ToCommand();

            Assert.Empty(command.Properties);
        }

        [Fact]
        public void Noob_negative_is_NoNoob()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            builder.Parameter(x => x.Something).Rename("Noob");
            var command = builder.ToCommand();

            Assert.Single(
                command.Properties,
                p => p.Identifier == "NoNoob"
            );
        }

        [Fact]
        public void NoFoo_negative_is_Foo()
        {
            var builder = new NewProvisionerCommandBuilder<NullableTest>();
            builder.Parameter(x => x.Something).Rename("NoFoo");
            var command = builder.ToCommand();

            var noSomething = Assert.Single(
                command.Properties,
                p => p.Identifier == "Foo"
            );
        }

        private class NullableTest : HarshProvisioner
        {
            [Parameter]
            public Boolean? Something { get; set; }
        }

        private class NonNullableTest : HarshProvisioner
        {
            [Parameter]
            public Boolean Something { get; set; }
        }

        private class WithParamSets : HarshProvisioner
        {
            [Parameter(ParameterSetName = "A")]
            [Parameter(ParameterSetName = "B")]
            [Parameter(ParameterSetName = "C")]
            public Boolean? Param { get; set; }
        }
    }
}

