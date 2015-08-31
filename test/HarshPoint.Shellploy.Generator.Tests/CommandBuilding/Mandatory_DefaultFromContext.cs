using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Tests.CommandBuilding
{
    public class Mandatory_DefaultFromContext : SeriloggedTest
    {
        public Mandatory_DefaultFromContext(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void Is_not_SMA_mandatory()
        {
            var builder = new NewProvisionerCommandBuilder<TestProv>();
            var command = builder.ToCommand();

            var param = Assert.Single(command.Properties);

            var synth = Assert.Single(
                param.ElementsOfType<PropertyModelSynthesized>()
            );

            var attr = Assert.Single(
                synth.Attributes,
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );

            Assert.DoesNotContain("Mandatory", attr.Properties.Keys);
        }

        private sealed class TestProv : HarshProvisioner
        {
            [DefaultFromContext]
            [Parameter(Mandatory = true)]
            public String Param { get; set; }
        }

    }
}
