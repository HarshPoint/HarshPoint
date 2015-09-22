using HarshPoint;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
using HarshPoint.Tests;
using System;
using System.CodeDom;
using Xunit;
using Xunit.Abstractions;

namespace CodeGen
{
    public class Conditional_fixed_property_assignment : SeriloggedTest
    {
        private readonly CodeConditionStatement _condition;
        private readonly CodeConditionStatement _innerCondition;

        public Conditional_fixed_property_assignment(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<Test>();
            builder.Parameter(x => x.Param).SetConditionalFixedValue(x =>
            {
                x.When(CodeLiteralExpression.Create(true), 42);
                x.When(CodeLiteralExpression.Create(false), 43);
                x.Else(44);
            });

            var command = builder.ToCommand();

            var visitor = new NewObjectAssignmentVisitor(TargetObject);
            visitor.Visit(command.Properties);

            var statements = visitor.Statements;

            var stmt = Assert.Single(statements);
            _condition = Assert.IsType<CodeConditionStatement>(stmt);

            var innerStmt = Assert.Single(_condition.FalseStatements);
            _innerCondition = Assert.IsType<CodeConditionStatement>(innerStmt);
        }

        [Fact]
        public void First_right_is_fixed_value()
        {
            var stmt = Assert.Single(_condition.TrueStatements);
            var assign = Assert.IsType<CodeAssignStatement>(stmt);

            var rhs = Assert.IsType<CodePrimitiveExpression>(
                assign.Right
            );

            Assert.Equal(42, rhs.Value);
        }

        [Fact]
        public void Second_right_is_fixed_value()
        {
            var stmt = Assert.Single(_innerCondition.TrueStatements);
            var assign = Assert.IsType<CodeAssignStatement>(stmt);

            var rhs = Assert.IsType<CodePrimitiveExpression>(
                assign.Right
            );

            Assert.Equal(43, rhs.Value);
        }

        [Fact]
        public void Else_right_is_fixed_value()
        {
            var stmt = Assert.Single(_innerCondition.FalseStatements);
            var assign = Assert.IsType<CodeAssignStatement>(stmt);

            var rhs = Assert.IsType<CodePrimitiveExpression>(
                assign.Right
            );

            Assert.Equal(44, rhs.Value);
        }

        [Fact]
        public void First_condition_is_set()
        {
            var expr = Assert.IsType<CodePrimitiveExpression>(_condition.Condition);

            Assert.Equal(true, expr.Value);
        }

        [Fact]
        public void Second_condition_is_set()
        {
            var expr = Assert.IsType<CodePrimitiveExpression>(_innerCondition.Condition);

            Assert.Equal(false, expr.Value);
        }

        private static readonly CodeExpression TargetObject
            = new CodeVariableReferenceExpression("target");

        private class Test
        {
            [Parameter]
            public String Param { get; set; }
        }
    }
}
