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
    public class Ignored_Parameter : SeriloggedTest
    {
        public Ignored_Parameter(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Is_ignored()
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.IgnoredParam).Ignore();

            var command = builder.ToCommand();
            Assert.Empty(command.Properties);
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String IgnoredParam { get; set; }
        }
    }
}
