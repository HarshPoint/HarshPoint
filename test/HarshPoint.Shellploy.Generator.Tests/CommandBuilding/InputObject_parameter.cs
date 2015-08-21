﻿using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.Tests;
using Xunit;
using Xunit.Abstractions;

namespace CommandBuilding
{
    public class InputObject_parameter : SeriloggedTest
    {
        private readonly ShellployCommandProperty _property;
        private readonly AttributeData _paramAttr;

        public InputObject_parameter(ITestOutputHelper output) : base(output)
        {
            var builder = new NewObjectCommandBuilder<EmptyProvisioner>();
            builder.HasInputObject = true;

            var command = builder.ToCommand();

            _property = Assert.Single(command.Properties);
            _paramAttr = Assert.Single(_property.ParameterAttributes);
        }

        [Fact]
        public void Has_InputObject()
        {
            Assert.True(_property.IsInputObject);
        }


        [Fact]
        public void Has_identifier()
        {
            Assert.Equal(
                CommandBuilder.InputObjectPropertyName,
                _property.Identifier
            );
        }

        [Fact]
        public void Has_name()
        {
            Assert.Equal(
                CommandBuilder.InputObjectPropertyName, 
                _property.PropertyName
            );
        }


        [Fact]
        public void Is_Positional()
        {
            Assert.True(_property.IsPositional);
        }

        [Fact]
        public void Has_no_ProvisionerType()
        {
            Assert.Null(_property.ProvisionerType);
        }

        [Fact]
        public void Has_Position()
        {
            var pos = Assert.Single(
                _paramAttr.NamedArguments, 
                na => na.Key == "Position"
            );

            Assert.Equal(0, pos.Value);
        }

        [Fact]
        public void Is_ValueFromPipeline()
        {
            var vfp = Assert.Single(
                _paramAttr.NamedArguments,
                na => na.Key == "ValueFromPipeline"
            );

            Assert.Equal(true, vfp.Value);
        }

        [Fact]
        public void Is_not_ValueFromPipelineByPropertyName()
        {
            Assert.DoesNotContain(
                _paramAttr.NamedArguments,
                na => na.Key == "ValueFromPipelineByPropertyName"
            );
        }



        private sealed class EmptyProvisioner : HarshProvisioner
        {
        }
    }
}
