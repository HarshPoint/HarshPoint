using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class With_multiple_parameter_sets : SeriloggedTest
    {
        public With_multiple_parameter_sets(ITestOutputHelper output) : base(output)
        {
            var builder = new CommandBuilder<TestProvisioner>();
            Command = builder.ToCommand();

            Assert.Equal(3, Command.Properties.Count);

            Common = Assert.Single(
                Command.Properties,
                p => p.Identifier == "Common"
            );

            SetA_Mandatory = Assert.Single(
                Command.Properties,
                p => p.Identifier == "SetA_Mandatory"
            );

            SetAB_MandatoryB = Assert.Single(
                Command.Properties,
                p => p.Identifier == "SetAB_MandatoryB"
            );
        }

        private ShellployCommandProperty Common { get; }
        private ShellployCommandProperty SetA_Mandatory { get; }
        private ShellployCommandProperty SetAB_MandatoryB { get; }
        internal ShellployCommand Command { get; }

        [Fact]
        public void Common_has_single_attribute()
        {
            var attr = Assert.Single(Common.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void SetA_Mandatory_is_mandatory()
        {
            var attr = Assert.Single(SetA_Mandatory.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var mandatory = Assert.Single(
                attr.NamedArguments,
                na => na.Key == "Mandatory"
            );
            Assert.Equal(true, mandatory.Value);
        }

        [Fact]
        public void SetA_Mandatory_is_set_A()
        {
            var attr = Assert.Single(SetA_Mandatory.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var psn = Assert.Single(
                attr.NamedArguments,
                na => na.Key == "ParameterSetName"
            );
            Assert.Equal("A", psn.Value);
        }

        [Fact]
        public void SetAB_MandatoryB_has_two_ParameterAttributes()
        {
            Assert.Equal(2, SetAB_MandatoryB.Attributes.Count);
            Assert.All(SetAB_MandatoryB.Attributes, a =>
                Assert.Equal(typeof(SMA.ParameterAttribute), a.AttributeType)
            );
        }

        [Fact]
        public void SetAB_MandatoryB_is_set_A()
        {
            var attr = Assert.Single(
                SetAB_MandatoryB.Attributes,
                a => Equals("A", a.NamedArguments.GetValueOrDefault("ParameterSetName"))
            );

            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void SetAB_MandatoryB_is_mandatory_in_set_B()
        {
            var attr = Assert.Single(
                SetAB_MandatoryB.Attributes,
                a => Equals("B", a.NamedArguments.GetValueOrDefault("ParameterSetName"))
            );

            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var mandatory = Assert.Single(
                attr.NamedArguments,
                na => na.Key == "Mandatory"
            );

            Assert.Equal(true, mandatory.Value);
        }

        [Fact]
        public void Doesnt_have_DefaultParameterSet()
        {
            var attr = Assert.Single(
                Command.Attributes,
                a => a.AttributeType == typeof(SMA.CmdletAttribute)
            );

            Assert.DoesNotContain(
                attr.NamedArguments,
                a => a.Key == "DefaultParameterSetName"
            );
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String Common { get; set; }

            [Parameter(Mandatory = true, ParameterSetName = "A")]
            public String SetA_Mandatory { get; set; }

            [Parameter(ParameterSetName = "A")]
            [Parameter(ParameterSetName = "B", Mandatory = true)]
            public String SetAB_MandatoryB { get; set; }
        }
    }
}
