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
        public void Then_has_positive_validation()
        {
            var cond = Assert.IsType<CodeConditionStatement>(
                _if.TrueStatements.Cast<CodeStatement>().First()
            );

            var propRef =
                Assert.IsType<CodePropertyReferenceExpression>(cond.Condition);

            Assert.Equal("IsPresent", propRef.PropertyName);

            var switchPropRef = Assert.IsType<CodePropertyReferenceExpression>(
                propRef.TargetObject
            );

            Assert.Equal("Param", switchPropRef.PropertyName);

            Assert.IsType<CodeThisReferenceExpression>(
                switchPropRef.TargetObject
            );

            Assert.IsType<CodeMethodReturnStatement>(cond.TrueStatements[1]);

            var stmt =
                Assert.IsType<CodeExpressionStatement>(cond.TrueStatements[0]);

            var call =
                Assert.IsType<CodeMethodInvokeExpression>(stmt.Expression);

            Assert.IsType<CodeThisReferenceExpression>(
                call.Method.TargetObject
            );

            Assert.Equal(
                "WriteExclusiveSwitchValidationError",
                call.Method.MethodName
            );

            Assert.Equal(
                "Param",
                Assert.IsType<CodePrimitiveExpression>(call.Parameters[0]).Value
            );

            Assert.Equal(
                "NoParam",
                Assert.IsType<CodePrimitiveExpression>(call.Parameters[1]).Value
            );
        }

        [Fact]
        public void Then_has_false_assignment()
        {
            var assign = Assert.IsType<CodeAssignStatement>(
                _if.TrueStatements.Cast<CodeStatement>().Last()
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
