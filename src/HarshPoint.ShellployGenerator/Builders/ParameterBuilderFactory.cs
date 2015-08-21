using System;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ParameterBuilderFactory<TProvisioner> :
        IChildParameterBuilderFactory<TProvisioner>
    {
        internal ParameterBuilderFactory(
            ParameterBuilderContainer container,
            String name
        )
        {
            Container = container;
            Name = name;
        }

        public ParameterBuilderFactory<TProvisioner> Ignore()
        {
            Set(new ParameterBuilderIgnored());
            return this;
        }

        public ParameterBuilderFactory<TProvisioner> Rename(String propertyName)
        {
            CommandBuilder.ValidateParameterName(propertyName);
            Set(new ParameterBuilderRenamed(propertyName));
            return this;
        }

        public ParameterBuilderFactory<TProvisioner> SetDefaultValue(Object value)
        {
            Set(new ParameterBuilderDefaultValue(value));
            return this;
        }

        public ParameterBuilderFactory<TProvisioner> SetFixedValue(Object value)
        {
            Set(new ParameterBuilderFixed(value));
            return this;
        }

        public ParameterBuilderFactory<TProvisioner> SynthesizeMandatory(
            Type parameterType
        )
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            return Synthesize(
                parameterType,
                new AttributeData(typeof(SMA.ParameterAttribute))
                {
                    NamedArguments = { ["Mandatory"] = true }
                }
            );
        }
        public ParameterBuilderFactory<TProvisioner> Synthesize(
            Type parameterType,
            params AttributeData[] attributeData
        )
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            if (attributeData == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeData));
            }

            Set(new ParameterBuilderSynthesized(
                Name,
                parameterType, 
                attributes: attributeData
            ));

            return this;
        }

        private void Set(ParameterBuilder parameter)
        {
            Container.Update(Name, parameter);
        }

        IChildParameterBuilderFactory<TProvisioner>
        IChildParameterBuilderFactory<TProvisioner>.Ignore()
            => Ignore();

        IChildParameterBuilderFactory<TProvisioner>
        IChildParameterBuilderFactory<TProvisioner>.SetFixedValue(Object value)
            => SetFixedValue(value);

        private ParameterBuilderContainer Container { get; }

        private String Name { get; }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderFactory<>));
    }
}
