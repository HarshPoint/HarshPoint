using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public sealed class InputObject_and_positional : SeriloggedTest
    {
        private readonly CommandBuilder<WithPositional> _builder;
        private readonly ShellployCommand _command;
        private readonly ShellployCommandProperty _inputObject;

        public InputObject_and_positional(ITestOutputHelper output) : base(output)
        {
            _builder = new CommandBuilder<WithPositional>();
            _builder.HasInputObject = true;
            _builder.PositionalParameter(x => x.Pos0);
            _builder.PositionalParameter(x => x.Pos1);

            _command = _builder.ToCommand();

            _inputObject = Assert.Single(
                _command.Properties,
                p => p.Identifier == "InputObject"
            );
        }

        [Fact]
        public void Is_last()
        {
            var last = _command.Properties.Last();
            Assert.Same(_inputObject, last);
            Assert.True(last.IsInputObject);
            Assert.True(last.IsPositional);
        }

        private sealed class WithPositional : HarshProvisioner
        {
            [Parameter]
            public String Pos1 { get; set; }

            [Parameter]
            public String Named { get; set; }

            [Parameter]
            public String Pos0 { get; set; }
        }


    }
}
