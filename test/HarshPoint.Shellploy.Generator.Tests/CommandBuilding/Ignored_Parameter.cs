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
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.IgnoredParam).Ignore();

            var command = builder.ToCommand();
            var prop = Assert.Single(command.Properties);

            var ignored = Assert.Single(
                prop.ElementsOfType<PropertyModelIgnored>()
            );
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String IgnoredParam { get; set; }
        }
    }
}
