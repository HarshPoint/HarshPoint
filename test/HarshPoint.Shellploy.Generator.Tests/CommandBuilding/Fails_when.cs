using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Fails_when : SeriloggedTest
    {
        public Fails_when(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Has_InputObject_parameter()
        {
            Assert.Throws<ArgumentException>(() =>
                new CommandBuilder<InputObjectProvisioner>()
            );
        }


        private sealed class InputObjectProvisioner : HarshProvisioner
        {
            [Parameter]
            public String InputObject { get; set; }
        }

    }
}
