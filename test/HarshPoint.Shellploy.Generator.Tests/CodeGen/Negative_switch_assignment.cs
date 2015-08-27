using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
using HarshPoint.Tests;
using System;
using System.CodeDom;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CodeGen
{
    public class Negative_switch_assignment : SeriloggedTest
    {
        private readonly CodeConditionStatement _if;

        public Negative_switch_assignment(ITestOutputHelper output) : base(output)
        {
            var builder = new NewProvisionerCommandBuilder<TestProv>();
            var command = builder.ToCommand();

            var visitor = new NewObjectAssignmentVisitor(TargetObject);
            visitor.Visit(command.Properties);

            _if = Assert.IsType<CodeConditionStatement>(
                visitor.Statements.Cast<CodeStatement>().Last()
            );
        }

        [Fact]
        public void Then_is_false_assignment()
        {
            var assign = Assert.IsType<CodeAssignStatement>(
                Assert.Single(_if.TrueStatements)
            );

            var lhs = Assert.IsType<CodePropertyReferenceExpression>(
                assign.Left
            );
            Assert.Same(TargetObject, lhs.TargetObject);
            Assert.Equal("Param", lhs.PropertyName);

            var rhs = Assert.IsType<CodePrimitiveExpression>(assign.Right);
            Assert.Equal(false, rhs.Value);
        }

        private static readonly CodeExpression TargetObject
            = new CodeVariableReferenceExpression("target");

        private class TestProv : HarshProvisioner
        {
            [Parameter]
            public Boolean? Param { get; set; }
        }
    }
}
