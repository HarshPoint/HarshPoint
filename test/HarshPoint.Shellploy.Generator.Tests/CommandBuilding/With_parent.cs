using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class With_parent : SeriloggedTest
    {
        private readonly NewObjectCommandBuilder<Parent> _parent;
        private readonly NewObjectCommandBuilder<Child> _child;

        public With_parent(ITestOutputHelper output) : base(output)
        {
            _parent = new NewObjectCommandBuilder<Parent>();

            _child = new NewObjectCommandBuilder<Child>();
            _child.AsChildOf<Parent>();

            var context = new CommandBuilderContext();
            context.Add(_parent);
            context.Add(_child);
        }

        [Fact]
        public void Has_own_property()
        {
            var cmd = _child.ToCommand();

            var prop = Assert.Single(
                cmd.Properties,
                p => p.Identifier == "ChildParam"
            );
        }

        [Fact]
        public void Has_parent_property()
        {
            var cmd = _child.ToCommand();

            var prop = Assert.Single(
                cmd.Properties,
                p => p.Identifier == "ParentParam"
            );
        }

        [Fact]
        public void Doesnt_have_parents_InputObject()
        {
            _parent.HasInputObject = true;

            var cmd = _child.ToCommand();
            Assert.DoesNotContain(
                cmd.Properties,
                p => p.HasElementsOfType<PropertyModelInputObject>()
            );
        }

        [Fact]
        public void Cannot_change_parent_type()
        {
            Assert.Throws<InvalidOperationException>(
                () => _child.AsChildOf<Object>()
            );
        }

        [Fact]
        public void Can_ignore_parent_param()
        {
            _child.AsChildOf<Parent>(
                p => p.Parameter(x => x.ParentParam).Ignore()
            );

            var command = _child.ToCommand();

            var prop = Assert.Single(
                command.Properties,
                p => p.Identifier == "ParentParam"
            );

            Assert.Single(
                prop.ElementsOfType<PropertyModelIgnored>()
            );
        }

        [Fact]
        public void Can_set_fixed_value_parent_param()
        {
            _child.AsChildOf<Parent>(
                p => p.Parameter(x => x.ParentParam).SetFixedValue("42")
            );

            var command = _child.ToCommand();

            var prop = Assert.Single(
                command.Properties,
                p => p.Identifier == "ParentParam"
            );

            var fixVal = Assert.Single(
                prop.ElementsOfType<PropertyModelFixed>()
            );

            Assert.Equal("42", fixVal.Value);
        }

        [Fact]
        public void Ignoring_parent_param_on_child_doesnt_modify_parent()
        {
            _child.AsChildOf<Parent>(
                p => p.Parameter(x => x.ParentParam).Ignore()
            );

            var parentCommand = _parent.ToCommand();

            Assert.Single(
                parentCommand.Properties,
                p => p.Identifier == "ParentParam"
            );
        }

        [Fact]
        public void Fixing_parent_param_on_child_doesnt_modify_parent()
        {
            _child.AsChildOf<Parent>(
                p => p.Parameter(x => x.ParentParam).SetFixedValue("42")
            );

            var parentCommand = _parent.ToCommand();

            var prop = Assert.Single(
                parentCommand.Properties,
                p => p.Identifier == "ParentParam"
            );

            Assert.Empty(
                prop.ElementsOfType<PropertyModelFixed>()
            );
        }

        public class Parent : HarshProvisioner
        {
            [Parameter]
            public String ParentParam { get; set; }
        }

        public class Child : HarshProvisioner
        {
            [Parameter]
            public String ChildParam { get; set; }
        }

    }
}
