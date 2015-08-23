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
    public class Renamed_property_assignment : SeriloggedTest
    {
        private readonly CodeAssignStatement _assign;

        public Renamed_property_assignment(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<Test>();
            builder.Parameter(x => x.Param).Rename("Renamed");

            var command = builder.ToCommand();

            var visitor = new NewObjectAssignmentVisitor(TargetObject);
            visitor.Visit(command.Properties);

            var statements = visitor.Statements;

            var stmt = Assert.Single(statements);
            _assign = Assert.IsType<CodeAssignStatement>(stmt);
        }

        [Fact]
        public void Right_is_renamed_property_reference()
        {
            var rhs = Assert.IsType<CodePropertyReferenceExpression>(
                _assign.Right
            );

            Assert.IsType<CodeThisReferenceExpression>(rhs.TargetObject);
            Assert.Equal("Renamed", rhs.PropertyName);
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
