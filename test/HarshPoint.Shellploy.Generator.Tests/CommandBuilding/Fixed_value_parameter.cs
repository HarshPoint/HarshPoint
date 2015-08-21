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
    public class Fixed_value_parameter : SeriloggedTest
    {
        public Fixed_value_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.FixedValueParam).SetFixedValue(42);

            var command = builder.ToCommand();
            Property = Assert.Single(command.Properties);
        }

        private ShellployCommandProperty Property { get; }

        [Fact]
        public void FixedValue_is_set()
        {
            Assert.Equal(42, Property.FixedValue);
        }

        [Fact]
        public void HasFixedValue_is_true()
        {
            Assert.True(Property.HasFixedValue);
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String FixedValueParam { get; set; }
        }
    }
}
