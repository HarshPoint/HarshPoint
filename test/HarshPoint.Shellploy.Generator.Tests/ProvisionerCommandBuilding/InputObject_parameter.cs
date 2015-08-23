using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;
using HarshPoint;

namespace ProvisionerCommandBuilding
{
    public class InputObject_parameter : SeriloggedTest
    {
        private readonly PropertyModel _property;
        private readonly PropertyModelSynthesized _synthesized;

        private readonly AttributeModel _paramAttr;

        public InputObject_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new NewProvisionerCommandBuilder<EmptyProvisioner>();
            builder.HasInputObject = true;

            var command = builder.ToCommand();

            _property = Assert.Single(command.Properties);

            _synthesized = Assert.Single(
                _property.ElementsOfType<PropertyModelSynthesized>()
            );

            _paramAttr = Assert.Single(
                _synthesized.Attributes.Where(
                    a => a.AttributeType == typeof(SMA.ParameterAttribute)
                )
            );
        }

        [Fact]
        public void Has_InputObject()
        {
            Assert.True(
                _property.HasElementsOfType<PropertyModelInputObject>()
            );
        }


        [Fact]
        public void Has_identifier()
        {
            Assert.Equal(
                "InputObject",
                _property.Identifier
            );
        }

        [Fact]
        public void Is_Positional()
        {
            Assert.True(_property.HasElementsOfType<PropertyModelPositional>());
        }

        [Fact]
        public void Has_Position()
        {
            var pos = Assert.Single(
                _paramAttr.Properties,
                na => na.Key == "Position"
            );

            Assert.Equal(0, pos.Value);
        }

        [Fact]
        public void Is_ValueFromPipeline()
        {
            var vfp = Assert.Single(
                _paramAttr.Properties,
                na => na.Key == "ValueFromPipeline"
            );

            Assert.Equal(true, vfp.Value);
        }

        [Fact]
        public void Is_not_ValueFromPipelineByPropertyName()
        {
            Assert.DoesNotContain(
                _paramAttr.Properties,
                na => na.Key == "ValueFromPipelineByPropertyName"
            );
        }

        [Fact]
        public void Fails_when_object_has_InputObject_property()
        {
            Assert.Throws<ArgumentException>(() =>
                new NewProvisionerCommandBuilder<InputObjectProvisioner>()
            );
        }

        [Fact]
        public void Cannot_rename_to_InputObject()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var builder = new NewProvisionerCommandBuilder<TestProvisioner>();
                builder.Parameter(x => x.RenamedParam).Rename("InputObject");
            });
        }

        [Fact]
        public void Cannot_synthesize()
        {
            var builder = new NewProvisionerCommandBuilder<EmptyProvisioner>();

            Assert.Throws<ArgumentException>(() =>
                builder.Parameter("InputObject").Synthesize(typeof(Int32))
            );
        }

        private sealed class InputObjectProvisioner : HarshProvisioner
        {
            [Parameter]
            public String InputObject { get; set; }
        }

        private sealed class TestProvisioner : HarshProvisioner
        {
            [Parameter]
            public String RenamedParam { get; set; }
        }

        private sealed class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
