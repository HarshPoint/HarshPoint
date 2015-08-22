using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

namespace CommandBuilding
{
    public class Positional_parameters : SeriloggedTest
    {
        private readonly NewObjectCommandBuilder<TestProvisioner> _builder;
        private readonly CommandModel _command;

        private readonly PropertyModel _named;
        private readonly PropertyModel _pos0;
        private readonly PropertyModel _pos1;

        public Positional_parameters(ITestOutputHelper output) : base(output)
        {
            _builder = new NewObjectCommandBuilder<TestProvisioner>();
            _builder.PositionalParameter(x => x.Pos0);
            _builder.PositionalParameter(x => x.Pos1);

            _command = _builder.ToCommand();

            _pos0 = Assert.Single(
                _command.Properties, 
                p => p.Identifier == "Pos0"
            );

            _pos1 = Assert.Single(
                _command.Properties, 
                p => p.Identifier == "Pos1"
            );

            _named = Assert.Single(
                _command.Properties,
                p => p.Identifier == "Named"
            );
        }

        [Fact]
        public void Are_sorted()
        {
            Assert.Same(_pos0, _command.Properties[0]);
            Assert.Same(_pos1, _command.Properties[1]);
            Assert.Same(_named, _command.Properties[2]);
        }

        [Fact]
        public void Pos0_has_position_0()
        {
            var attr = Assert.Single(GetParameterAttributes(_pos0));

            var pos = Assert.Single(
                attr.Properties, 
                na => na.Key == "Position"
            );

            Assert.Equal(0, pos.Value);
        }

        [Fact]
        public void Pos0_is_positional()
        {
            Assert.True(_pos0.HasElementsOfType<PropertyModelPositional>());
        }

        [Fact]
        public void Pos1_has_position_1()
        {
            var attr = Assert.Single(GetParameterAttributes(_pos1));

            var pos = Assert.Single(
                attr.Properties,
                na => na.Key == "Position"
            );

            Assert.Equal(1, pos.Value);
        }


        [Fact]
        public void Pos1_is_positional()
        {
            Assert.True(_pos1.HasElementsOfType<PropertyModelPositional>());
        }


        [Fact]
        public void Named_is_not_positional()
        {
            Assert.False(_named.HasElementsOfType<PropertyModelPositional>());
        }


        [Fact]
        public void Named_has_no_position()
        {
            var attr = Assert.Single(GetParameterAttributes(_named));

            Assert.DoesNotContain(
                attr.Properties,
                na => na.Key == "Position"
            );
        }

        private static IEnumerable<AttributeModel> GetParameterAttributes(
            PropertyModel property
        )
        {
            var synth = Assert.Single(
                property.ElementsOfType<PropertyModelSynthesized>()
            );

            return synth.Attributes.Where(
                a => a.AttributeType == typeof(SMA.ParameterAttribute)
            );
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public Int32 Pos1 { get; set; }

            [Parameter]
            public String Named { get; set; }

            [Parameter]
            public Int32 Pos0 { get; set; }
        }
    }
}
