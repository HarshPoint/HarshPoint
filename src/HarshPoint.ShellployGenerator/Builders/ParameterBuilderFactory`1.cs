using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderFactory<TProvisioner> :
        ParameterBuilderFactory, 
        IChildParameterBuilderFactory<TProvisioner>
    {
        internal ParameterBuilderFactory(
            ParameterBuilderContainer container,
            String name
        )
            : base(container, name)
        {
        }

        public new ParameterBuilderFactory<TProvisioner> Ignore()
        {
            base.Ignore();
            return this;
        }

        public new ParameterBuilderFactory<TProvisioner> Rename(String propertyName)
        {
            base.Rename(propertyName);
            return this;
        }

        public new ParameterBuilderFactory<TProvisioner> SetDefaultValue(Object value)
        {
            base.SetDefaultValue(value);
            return this;
        }

        public new ParameterBuilderFactory<TProvisioner> SetFixedValue(Object value)
        {
            base.SetFixedValue(value);
            return this;
        }

        public new ParameterBuilderFactory<TProvisioner> SynthesizeMandatory(
            Type parameterType
        )
        {
            base.SynthesizeMandatory(parameterType);
            return this;
        }

        public new ParameterBuilderFactory<TProvisioner> Synthesize(
            Type parameterType,
            params AttributeData[] attributeData
        )
        {
            base.Synthesize(parameterType, attributeData);
            return this;
        }

        IChildParameterBuilderFactory<TProvisioner>
        IChildParameterBuilderFactory<TProvisioner>.Ignore()
            => Ignore();

        IChildParameterBuilderFactory<TProvisioner>
        IChildParameterBuilderFactory<TProvisioner>.SetFixedValue(Object value)
            => SetFixedValue(value);
    }
}
