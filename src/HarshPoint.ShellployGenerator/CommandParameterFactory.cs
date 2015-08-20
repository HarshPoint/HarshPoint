using HarshPoint.Provisioning.Implementation;
using System;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterFactory<TProvisioner>
        where TProvisioner : HarshProvisionerBase
    {
        internal CommandParameterFactory(
            CommandBuilder<TProvisioner> builder,
            String name
        )
        {
            Builder = builder;
            Name = name;
        }

        public String Name { get; }

        public CommandParameterFactory<TProvisioner> Ignore()
        {
            Set(new CommandParameterIgnored());
            return this;
        }

        public CommandParameterFactory<TProvisioner> Rename(String propertyName)
        {
            Builder.ValidateParameterName(propertyName);
            Set(new CommandParameterRenamed(propertyName));
            return this;
        }

        public CommandParameterFactory<TProvisioner> SetDefaultValue(Object value)
        {
            Set(new CommandParameterDefaultValue(value));
            return this;
        }

        public CommandParameterFactory<TProvisioner> SetFixedValue(Object value)
        {
            Set(new CommandParameterFixed(value));
            return this;
        }

        public CommandParameterFactory<TProvisioner> SynthesizeMandatory(
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
        public CommandParameterFactory<TProvisioner> Synthesize(
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

            Set(new CommandParameterSynthesized(
                Name,
                parameterType, 
                attributes: attributeData
            ));

            return this;
        }

        private CommandBuilder<TProvisioner> Builder { get; }

        private void Set(CommandParameter parameter)
        {
            Builder.SetParameter(Name, parameter);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandParameterFactory<>));
    }
}
