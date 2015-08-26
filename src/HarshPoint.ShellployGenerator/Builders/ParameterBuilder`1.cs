using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilder<TProvisioner> :
        ParameterBuilder, 
        IChildParameterBuilder<TProvisioner>
    {
        internal ParameterBuilder(
            PropertyModelContainer container,
            String name
        )
            : base(container, name)
        {
        }

        public new ParameterBuilder<TProvisioner> Ignore()
        {
            base.Ignore();
            return this;
        }

        public new ParameterBuilder<TProvisioner> NoNegative()
        {
            base.NoNegative();
            return this;
        }

        public new ParameterBuilder<TProvisioner> Rename(String propertyName)
        {
            base.Rename(propertyName);
            return this;
        }

        public new ParameterBuilder<TProvisioner> SetDefaultValue(Object value)
        {
            base.SetDefaultValue(value);
            return this;
        }

        public new ParameterBuilder<TProvisioner> SetFixedValue(Object value)
        {
            base.SetFixedValue(value);
            return this;
        }

        public new ParameterBuilder<TProvisioner> SynthesizeMandatory(
            Type parameterType
        )
        {
            base.SynthesizeMandatory(parameterType);
            return this;
        }

        public new ParameterBuilder<TProvisioner> Synthesize(
            Type parameterType,
            params AttributeBuilder[] attributes
        )
        {
            base.Synthesize(parameterType, attributes);
            return this;
        }
        public new ParameterBuilder<TProvisioner> Synthesize(
            Type parameterType,
            IEnumerable<AttributeModel> attributes
        )
        {
            base.Synthesize(parameterType, attributes);
            return this;
        }

        IChildParameterBuilder<TProvisioner>
        IChildParameterBuilder<TProvisioner>.Ignore()
            => Ignore();

        IChildParameterBuilder<TProvisioner>
        IChildParameterBuilder<TProvisioner>.SetFixedValue(Object value)
            => SetFixedValue(value);
    }
}
