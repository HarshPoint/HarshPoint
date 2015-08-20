using HarshPoint;
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
    public class Default_parameter_set : SeriloggedTest
    {
        private readonly ShellployCommand _command;

        public Default_parameter_set(ITestOutputHelper output) : base(output)
        {
            _command = new CommandBuilder<ParameterSetProvisioner>().ToCommand();
        }

        private Boolean WithAttributeType<T>(AttributeData attr)
            => attr.AttributeType == typeof(T);

        [Fact]
        public void Has_default_parameter_set_name()
        {
            var attr = Assert.Single(
                _command.Attributes,
                WithAttributeType<SMA.CmdletAttribute>
            );
            Assert.Equal("ParamSet2", attr.NamedArguments["DefaultParameterSetName"]);
        }

        [DefaultParameterSet("ParamSet2")]
        private class ParameterSetProvisioner : HarshProvisioner
        {
            [Parameter(ParameterSetName = "ParamSet1")]
            public String Param1 { get; set; }

            [Parameter(ParameterSetName = "ParamSet2")]
            public String Param2 { get; set; }
        }
    }
}
