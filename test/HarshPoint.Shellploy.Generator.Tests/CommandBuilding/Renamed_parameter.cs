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
        public Renamed_parameter(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void Can_rename_nonexistent_param()
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Parameter("DoesNotExist").Rename("StillDoesNot");

            var command = builder.ToCommand();
            var property = Assert.Single(command.Properties);

            Assert.Equal("RenamedParam", property.PropertyName);
        }

        [Fact]
        public void Has_new_PropertyName()
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.RenamedParam).Rename("NewName");

            var command = builder.ToCommand();
            var property = Assert.Single(command.Properties);

            Assert.Equal("NewName", property.PropertyName);
        }

        [Fact]
        public void Has_original_Identifier()
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.RenamedParam).Rename("NewName");

            var command = builder.ToCommand();
            var property = Assert.Single(command.Properties);

            Assert.Equal("RenamedParam", property.Identifier);
        }

        [Fact]
        public void Cannot_rename_to_null()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new CommandBuilder<TestProvisioner>();
                builder.Parameter(x => x.RenamedParam).Rename(null);
            });
        }

        [Fact]
        public void Cannot_rename_to_empty_string()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new CommandBuilder<TestProvisioner>();
                builder.Parameter(x => x.RenamedParam).Rename(String.Empty);
            });
        }


        [Fact]
        public void Cannot_rename_to_whitespace_string()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new CommandBuilder<TestProvisioner>();
                builder.Parameter(x => x.RenamedParam).Rename(" \t \r \n ");
            });
        }

        [Fact]
        public void Cannot_rename_to_InputObject()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new CommandBuilder<TestProvisioner>();
                builder.Parameter(x => x.RenamedParam).Rename("InputObject");
            });
        }


        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String RenamedParam { get; set; }
        }
    }
}
