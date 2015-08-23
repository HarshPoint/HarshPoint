using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace ProvisionerCommandBuilding
{
    public class Basic_parameter : SeriloggedTest
    {
        private readonly PropertyModel _property;
        public Basic_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new NewProvisionerCommandBuilder<TestProvisioner>();
            var command = builder.ToCommand();

            _property = Assert.Single(
                command.Properties,
                p => p.Identifier == "BasicParam"
            );
        }

        [Fact]
        public void Is_ValueFromPipelineByPropertyName()
        {
            var synth = Assert.Single(
                _property.ElementsOfType<PropertyModelSynthesized>()
            );

            var attr = Assert.Single(
                synth.Attributes,
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );

            var namedArg = Assert.Single(attr.Properties);
            Assert.Equal("ValueFromPipelineByPropertyName", namedArg.Key);
            Assert.Equal(true, namedArg.Value);
        }
        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter()]
            public String BasicParam { get; set; }
        }
    }
}