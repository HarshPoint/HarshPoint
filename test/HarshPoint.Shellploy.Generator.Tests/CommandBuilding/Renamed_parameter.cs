using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Renamed_parameter : SeriloggedTest
    {
        private readonly ShellployCommandProperty _property;

        public Renamed_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.RenamedParam).Rename("NewName");

            var command = builder.ToCommand();
            _property = Assert.Single(command.Properties);
        }

        [Fact]
        public void Has_new_PropertyName()
        {
            Assert.Equal("NewName", _property.PropertyName);
        }

        [Fact]
        public void Has_original_Identifier()
        {
            Assert.Equal("RenamedParam", _property.Identifier);
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String RenamedParam { get; set; }
        }
    }
}
