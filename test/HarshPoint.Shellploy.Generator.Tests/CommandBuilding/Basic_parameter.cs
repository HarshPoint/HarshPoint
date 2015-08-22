using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
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
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            var command = builder.ToCommand();

            Property = Assert.Single(
                command.Properties,
                p => p.Identifier == "BasicParam"
            );
        }

        private PropertyModel Property { get; }


        [Fact]
        public void Has_Identifier()
        {
            Assert.Equal("BasicParam", Property.Identifier);
        }


        [Fact]
        public void Has_no_SortOrder()
        {
            Assert.Null(Property.SortOrder);
        }

        [Fact]
        public void Is_not_positional()
        {
            Assert.False(
                Property.HasElementsOfType<PropertyModelPositional>()
            );
        }

        [Fact]
        public void Has_no_default_value()
        {
            Assert.False(
                Property.HasElementsOfType<PropertyModelDefaultValue>()
            );
        }

        [Fact]
        public void Has_no_fixed_value()
        {
            Assert.False(
                Property.HasElementsOfType<PropertyModelFixed>()
            );
        }

        [Fact]
        public void Has_PropertyType()
        {
            var synth = Assert.Single(
                Property.ElementsOfType<PropertyModelSynthesized>()
            );
            Assert.Equal(typeof(String), synth.PropertyType);
        }

        [Fact]
        public void Has_Parameter_Attribute()
        {
            var synth = Assert.Single(
                Property.ElementsOfType<PropertyModelSynthesized>()
            );
            var attr = Assert.Single(synth.Attributes);
            Assert.Equal(typeof(SMA.ParameterAttribute), attr.AttributeType);
        }

        [Fact]
        public void Is_ValueFromPipelineByPropertyName()
        {
            var synth = Assert.Single(
                Property.ElementsOfType<PropertyModelSynthesized>()
            );

            var attr = Assert.Single(
                synth.Attributes,
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );

            var namedArg = Assert.Single(attr.Properties);
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