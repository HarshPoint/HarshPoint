using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Default_value_Parameter : SeriloggedTest
    {
        public Default_value_Parameter(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Default_value_is_set()
        {
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.DefaultValueParam).SetDefaultValue(42);

            var command = builder.ToCommand();
            var property = Assert.Single(command.Properties);

            var defVal = Assert.Single(
                property.ElementsOfType<PropertyModelDefaultValue>()
            );

            Assert.Equal(42, defVal.DefaultValue);
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String DefaultValueParam { get; set; }
        }
    }
}
