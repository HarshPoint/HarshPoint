using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class Basic_parameter : SeriloggedTest
    {
        public Basic_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new CommandBuilder<TestProvisioner>();
            var command = builder.ToCommand();

            Property = Assert.Single(
                command.Properties,
                p => p.Identifier == "BasicParam"
            );
        }

        private ShellployCommandProperty Property { get; }

        [Fact]
        public void Is_not_positional()
        {
            Assert.False(Property.IsPositional);
        }

        [Fact]
        public void Has_no_default_value()
        {
            Assert.Null(Property.DefaultValue);
        }

        [Fact]
        public void Has_no_fixed_value()
        {
            Assert.Null(Property.FixedValue);
            Assert.False(Property.HasFixedValue);
        }

        [Fact]
        public void Has_default_PropertyName()
        {
            Assert.Equal("BasicParam", Property.PropertyName);
        }

        [Fact]
        public void Has_ProvisionerType()
        {
            Assert.Equal(typeof(TestProvisioner), Property.ProvisionerType);
        }

        [Fact]
        public void Has_Type()
        {
            Assert.Equal(typeof(String), Property.Type);
        }

        [Fact]
        public void Has_Parameter_Attribute()
        {
            var attr = Assert.Single(Property.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void Is_ValueFromPipelineByPropertyName()
        {
            var attr = Assert.Single(
                Property.Attributes,
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );

            var namedArg = Assert.Single(attr.NamedArguments);
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