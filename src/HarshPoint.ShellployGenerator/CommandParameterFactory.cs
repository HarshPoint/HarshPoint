using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterFactory<TProvisioner>
        where TProvisioner : HarshProvisionerBase
    {
        internal CommandParameterFactory(
            CommandBuilder<TProvisioner> builder,
            String name,
            Boolean isPositional
        )
        {
            Builder = builder;
            Name = name;
            IsPositional = isPositional;
        }

        public String Name { get; }

        public CommandParameterFactory<TProvisioner> Ignore()
        {
            Set(new CommandParameterIgnored());
            return this;
        }

        public CommandParameterFactory<TProvisioner> Rename(String propertyName)
        {
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

        public CommandParameterFactory<TProvisioner> Synthesize(Type parameterType)
        {
            Synthesize(parameterType, new AttributeData[0]);
            return this;
        }

        public CommandParameterFactory<TProvisioner> Synthesize(
            PropertyInfo property,
            params AttributeData[] attributeData
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (attributeData == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeData));
            }

            Set(new CommandParameterSynthesized(
                property.PropertyType,
                property.DeclaringType,
                attributeData
            ));

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
                    NamedArguments =
                    {
                        ["Mandatory"] = true
                    }
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

            Set(new CommandParameterSynthesized(parameterType, attributeData));
            return this;
        }

        private CommandBuilder<TProvisioner> Builder { get; }

        private Boolean IsPositional { get; }

        private void Set(CommandParameter parameter)
        {
            Builder.SetParameter(Name, IsPositional, parameter);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandParameterFactory<>));
    }
}
