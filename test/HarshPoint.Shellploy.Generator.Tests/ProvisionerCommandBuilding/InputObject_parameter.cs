using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using SMA = System.Management.Automation;

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
                CommandBuilder.InputObjectIdentifier,
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

        private sealed class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
