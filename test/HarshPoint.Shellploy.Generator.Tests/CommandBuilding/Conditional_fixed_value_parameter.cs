using HarshPoint;
using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class Conditional_fixed_value_parameter : SeriloggedTest
    {
        public Conditional_fixed_value_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<TestProvisioner>();
            builder.Parameter(x => x.ConditionalFixedValueParam).SetConditionalFixedValue(x =>
            {
                x.When(CodeLiteralExpression.Create(true), 42);
                x.When(CodeLiteralExpression.Create(false), 43);
                x.Else(44);
            });

            var command = builder.ToCommand();
            Property = Assert.Single(command.Properties);
        }

        private PropertyModel Property { get; }

        [Fact]
        public void FixedValue_is_set()
        {
            var fix = Assert.Single(
                Property.ElementsOfType<PropertyModelConditionalFixed>()
            );

            Assert.Equal(2, fix.ConditionalValues.Count);
            Assert.Equal(42, fix.ConditionalValues[0].Item2);
            Assert.Equal(43, fix.ConditionalValues[1].Item2);
            Assert.Equal(44, fix.ElseValue);
        }

        [Fact]
        public void Has_FixedValue()
        {
            Assert.True(Property.HasElementsOfType<PropertyModelConditionalFixed>());
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String ConditionalFixedValueParam { get; set; }
        }
    }
}
