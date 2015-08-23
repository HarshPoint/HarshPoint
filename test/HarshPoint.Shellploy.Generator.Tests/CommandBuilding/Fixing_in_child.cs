using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Fixing_in_child : SeriloggedTest
    {
        private readonly NewProvisionerCommandBuilder<ChildProv> _childBuilder;
        private readonly NewProvisionerCommandBuilder<ParentProv> _parentBuilder;
        private readonly CommandModel _cmd;

        public Fixing_in_child(ITestOutputHelper output) : base(output)
        {
            _childBuilder = new NewProvisionerCommandBuilder<ChildProv>();
            _parentBuilder = new NewProvisionerCommandBuilder<ParentProv>();

            var context = new CommandBuilderContext();
            context.Add(_childBuilder);
            context.Add(_parentBuilder);

            _childBuilder.AsChildOf<ParentProv>(
                    p => p.Parameter(x => x.ParentA).SetFixedValue("fixed")
                );

            _cmd = _childBuilder.ToCommand();
        }



        [Fact]
        public void Ignores_other_parent_param()
        {
            Assert.DoesNotContain(
                _cmd.Properties,
                p => p.Identifier == "ParentB"
            );
        }


        [Fact]
        public void Keeps_child_params()
        {
            Assert.Single(
                _cmd.Properties,
                p => p.Identifier == "ChildA"
            );

            Assert.Single(
                _cmd.Properties,
                p => p.Identifier == "ChildB"
            );
        }

        private sealed class ParentProv : HarshProvisioner
        {
            [Parameter(ParameterSetName = "A")]
            public String ParentA { get; set; }

            [Parameter(ParameterSetName = "B")]
            public String ParentB { get; set; }
        }

        private sealed class ChildProv : HarshProvisioner
        {
            [Parameter(ParameterSetName = "A")]
            public String ChildA { get; set; }

            [Parameter(ParameterSetName = "B")]
            public String ChildB { get; set; }
        }
    }
}
