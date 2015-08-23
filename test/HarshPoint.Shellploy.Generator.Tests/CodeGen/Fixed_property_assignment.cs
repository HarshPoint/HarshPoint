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
    public class Fixed_property_assignment : SeriloggedTest
    {
        private readonly CodeAssignStatement _assign;

        public Fixed_property_assignment(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<Test>();
            builder.Parameter(x => x.Param).SetFixedValue("42");

            var command = builder.ToCommand();

            var visitor = new NewObjectAssignmentVisitor(TargetObject);
            visitor.Visit(command.Properties);

            var statements = visitor.Statements;

            var stmt = Assert.Single(statements);
            _assign = Assert.IsType<CodeAssignStatement>(stmt);
        }

        [Fact]
        public void Right_is_fixed_value()
        {
            var rhs = Assert.IsType<CodePrimitiveExpression>(
                _assign.Right
            );

            Assert.Equal("42", rhs.Value);
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
