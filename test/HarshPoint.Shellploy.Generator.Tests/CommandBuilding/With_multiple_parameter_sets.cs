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
        private readonly CommandModel _command;

        private readonly PropertyModel _common;
        private readonly PropertyModelSynthesized _commonSynth;

        private readonly PropertyModel _setA_Mandatory;
        private readonly PropertyModelSynthesized _setA_MandatorySynth;

        private readonly PropertyModel _setAB_MandatoryB;
        private readonly PropertyModelSynthesized _setAB_MandatoryBSynth;

        public With_multiple_parameter_sets(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            _command = builder.ToCommand();

            Assert.Equal(3, _command.Properties.Length);

            _common = Assert.Single(
                _command.Properties,
                p => p.Identifier == "Common"
            );

            _commonSynth = Assert.Single(
                _common.ElementsOfType<PropertyModelSynthesized>()
            );

            _setA_Mandatory = Assert.Single(
                _command.Properties,
                p => p.Identifier == "SetA_Mandatory"
            );

            _setA_MandatorySynth = Assert.Single(
                _setA_Mandatory.ElementsOfType<PropertyModelSynthesized>()
            );

            _setAB_MandatoryB = Assert.Single(
                _command.Properties,
                p => p.Identifier == "SetAB_MandatoryB"
            );

            _setAB_MandatoryBSynth = Assert.Single(
                _setAB_MandatoryB.ElementsOfType<PropertyModelSynthesized>()
            );
        }

        [Fact]
        public void Common_has_single_attribute()
        {
            var attr = Assert.Single(_commonSynth.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void SetA_Mandatory_is_mandatory()
        {
            var attr = Assert.Single(_setA_MandatorySynth.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var mandatory = Assert.Single(
                attr.Properties,
                na => na.Key == "Mandatory"
            );
            Assert.Equal(true, mandatory.Value);
        }

        [Fact]
        public void SetA_Mandatory_is_set_A()
        {
            var attr = Assert.Single(_setA_MandatorySynth.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var psn = Assert.Single(
                attr.Properties,
                na => na.Key == "ParameterSetName"
            );
            Assert.Equal("A", psn.Value);
        }

        [Fact]
        public void SetAB_MandatoryB_has_two_ParameterAttributes()
        {
            Assert.Equal(2, _setAB_MandatoryBSynth.Attributes.Length);
            Assert.All(_setAB_MandatoryBSynth.Attributes, a =>
                Assert.Equal(typeof(SMA.ParameterAttribute), a.AttributeType)
            );
        }

        [Fact]
        public void SetAB_MandatoryB_is_set_A()
        {
            var attr = Assert.Single(
                _setAB_MandatoryBSynth.Attributes,
                a => Equals("A", a.Properties["ParameterSetName"])
            );

            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void SetAB_MandatoryB_is_mandatory_in_set_B()
        {
            var attr = Assert.Single(
                _setAB_MandatoryBSynth.Attributes,
                a => Equals("B", a.Properties["ParameterSetName"])
            );

            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);

            var mandatory = Assert.Single(
                attr.Properties,
                na => na.Key == "Mandatory"
            );

            Assert.Equal(true, mandatory.Value);
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
