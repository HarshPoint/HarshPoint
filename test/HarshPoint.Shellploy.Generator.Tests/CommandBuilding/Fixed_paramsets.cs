using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Fixed_paramsets : SeriloggedTest
    {
        public Fixed_paramsets(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void ParamA_and_Param_B_cant_both_be_fixed()
        {
            var builder = new NewObjectCommandBuilder<Test>();
            builder.Parameter(x => x.Param_SetA).SetFixedValue("AAA");
            builder.Parameter(x => x.Param_SetB).SetFixedValue("BBB");

            var visitor = new IgnoreUnfixedParameterSetPropertiesVisitor();

            Assert.Throws<InvalidOperationException>(
                () => visitor.Visit(builder.PropertyContainer)
            );
        }

        [Fact]
        public void Fixing_ParamA_ignores_ParamB()
        {
            var builder = new NewObjectCommandBuilder<Test>();
            builder.Parameter(x => x.Param_SetA).SetFixedValue("AAA");

            var visitor = new IgnoreUnfixedParameterSetPropertiesVisitor();
            var result = visitor.Visit(builder.PropertyContainer);

            var paramA = Assert.Single(
                result,
                p => p.Identifier == "Param_SetA"
            );

            var paramB = Assert.Single(
                result,
                p => p.Identifier == "Param_SetB"
            );

            Assert.True(paramA.HasElementsOfType<PropertyModelFixed>());
            Assert.True(paramB.HasElementsOfType<PropertyModelIgnored>());
        }

        private class Test
        {
            [Parameter(ParameterSetName = "A")]
            public String Param_SetA { get; set; }

            [Parameter(ParameterSetName = "B")]
            public String Param_SetB { get; set; }
        }
    }
}
