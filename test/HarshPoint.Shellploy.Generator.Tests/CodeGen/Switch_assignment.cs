using HarshPoint;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
using HarshPoint.Tests;
using System;
using System.CodeDom;
using Xunit;
using Xunit.Abstractions;

namespace CodeGen
{
    public class Switch_assignment : SeriloggedTest
    {
        private readonly CodeConditionStatement _if;

        public Switch_assignment(ITestOutputHelper output) : base(output)
        {
            var builder = new NewProvisionerCommandBuilder<Test>();
            var command = builder.ToCommand();

            var visitor = new NewObjectAssignmentVisitor(TargetObject);
            visitor.Visit(command.Properties);

            var stmt = Assert.Single(visitor.Statements);
            _if = Assert.IsType<CodeConditionStatement>(stmt);
        }


        [Fact]
        public void Condition_is_switch_property_IsPresent()
        {
            var propRef = Assert.IsType<CodePropertyReferenceExpression>(
                _if.Condition
            );

            Assert.Equal("IsPresent", propRef.PropertyName);

            var lhs = Assert.IsType<CodePropertyReferenceExpression>(
                propRef.TargetObject
            );

            Assert.IsType<CodeThisReferenceExpression>(lhs.TargetObject);
            Assert.Equal("Param", lhs.PropertyName);
        }

        [Fact]
        public void Then_is_true_assignment()
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
            Assert.Equal(true, rhs.Value);
        }


        private static readonly CodeExpression TargetObject
            = new CodeVariableReferenceExpression("target");

        private class Test : HarshPoint.Provisioning.HarshProvisioner
        {
            [Parameter]
            public Boolean Param { get; set; }
        }

    }
}
