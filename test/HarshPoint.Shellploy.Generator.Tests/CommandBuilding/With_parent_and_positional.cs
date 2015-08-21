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
    public class With_parent_and_positional : SeriloggedTest
    {
        private readonly CommandBuilder<Parent> _parent;
        private readonly CommandBuilder<Child> _child;
        private readonly ShellployCommand _childCommand;

        public With_parent_and_positional(ITestOutputHelper output) : base(output)
        {
            _parent = new CommandBuilder<Parent>();
            _parent.PositionalParameter(x => x.Parent0);
            _parent.PositionalParameter(x => x.Parent1);

            _child = new CommandBuilder<Child>();
            _child.AsChildOf<Parent>();
            _child.PositionalParameter(x => x.Child0);
            _child.PositionalParameter(x => x.Child1);

            var context = new CommandBuilderContext();
            context.Add(_parent);
            context.Add(_child);

            _childCommand = _child.ToCommand();
        }


        [Fact]
        public void Parent_positional_go_before_child()
        {
            var command = _child.ToCommand();

            AssertParamPosition(0, "Parent0");
            AssertParamPosition(1, "Parent1");
            AssertParamPosition(2, "Child0");
            AssertParamPosition(3, "Child1");
        }


        private void AssertParamPosition(
            Int32 expectedPosition, 
            String propertyId
        )
        {
            var param = Assert.Single(
                _childCommand.Properties,
                p => p.Identifier == propertyId
            );

            var attr = Assert.Single(param.ParameterAttributes);

            var actualPosition = 
                (Int32?)attr.NamedArguments.GetValueOrDefault("Position");

            Assert.Equal(expectedPosition, actualPosition);
        }

        private sealed class Parent : HarshProvisioner
        {
            [Parameter]
            public String Parent1 { get; set; }

            [Parameter]
            public String ParentNamed { get; set; }

            [Parameter]
            public String Parent0 { get; set; }
        }

        private sealed class Child :HarshProvisioner
        {
            [Parameter]
            public String Child1 { get; set; }

            [Parameter]
            public String ChildNamed { get; set; }

            [Parameter]
            public String Child0 { get; set; }
        }
    }
}
