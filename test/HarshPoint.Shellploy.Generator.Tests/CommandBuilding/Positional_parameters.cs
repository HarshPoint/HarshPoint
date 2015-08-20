using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Positional_parameters : SeriloggedTest
    {
        private readonly ShellployCommandProperty _named;
        private readonly ShellployCommandProperty _pos0;
        private readonly ShellployCommandProperty _pos1;
        private readonly ShellployCommand _command;

        public Positional_parameters(ITestOutputHelper output) : base(output)
        {
            var builder = new CommandBuilder<TestProvisioner>();
            builder.PositionalParameter(x => x.Pos0);
            builder.PositionalParameter(x => x.Pos1);

            _command = builder.ToCommand();

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
            var attr = Assert.Single(_pos0.ParameterAttributes);

            var pos = Assert.Single(
                attr.NamedArguments, 
                na => na.Key == "Position"
            );

            Assert.Equal(0, pos.Value);
        }


        [Fact]
        public void Pos0_is_positional()
        {
            Assert.True(_pos0.IsPositional);
        }

        [Fact]
        public void Pos1_has_position_1()
        {
            var attr = Assert.Single(_pos1.ParameterAttributes);

            var pos = Assert.Single(
                attr.NamedArguments,
                na => na.Key == "Position"
            );

            Assert.Equal(1, pos.Value);
        }


        [Fact]
        public void Pos1_is_positional()
        {
            Assert.True(_pos1.IsPositional);
        }

        [Fact]
        public void Named_has_no_position()
        {
            var attr = Assert.Single(_named.ParameterAttributes);

            Assert.DoesNotContain(
                attr.NamedArguments,
                na => na.Key == "Position"
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
