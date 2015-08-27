using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.ShellployGenerator.Tests.CommandBuilding
{
    public class Changed_PropertyType : SeriloggedTest
    {
        private readonly NewObjectCommandBuilder<Target> _builder;
        private readonly CommandModel _command;
        private readonly ChangePropertyTypeVisitor _visitor;

        public Changed_PropertyType(ITestOutputHelper output) : base(output)
        {
            _builder = new NewObjectCommandBuilder<Target>();
            _visitor = new ChangePropertyTypeVisitor(
                typeof(String), typeof(Int32)
            );
            _command = _builder.ToCommand();
        }


        [Fact]
        public void Changes_from_FromType_to_ToType()
        {
            var properties = _visitor.Visit(_command.Properties);

            var stringParam = Assert.Single(
                properties,
                p => p.Identifier == "StringParam"
            );

            var synth = Assert.Single(
                stringParam.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(Int32), synth.PropertyType);
        }


        [Fact]
        public void Doesnt_change_other_properties()
        {
            var properties = _visitor.Visit(_command.Properties);

            var boolParam = Assert.Single(
                properties,
                p => p.Identifier == "BooleanParam"
            );

            var synth = Assert.Single(
                boolParam.ElementsOfType<PropertyModelSynthesized>()
            );

            Assert.Equal(typeof(Boolean), synth.PropertyType);
        }
        private sealed class Target
        {
            [Parameter]
            public String StringParam { get; set; }

            [Parameter]
            public Boolean BooleanParam { get; set; }
        }

    }
}
