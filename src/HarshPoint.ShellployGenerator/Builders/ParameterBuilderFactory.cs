using System;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class ParameterBuilderFactory
        : IChildParameterBuilderFactory
    {
        internal ParameterBuilderFactory(
            ParameterBuilderContainer container,
            String name
        )
        {
            Container = container;
            Name = name;
        }

        public ParameterBuilderFactory Ignore()
        {
            Set(new ParameterBuilderIgnored());
            return this;
        }

        public ParameterBuilderFactory Rename(String propertyName)
        {
            CommandBuilder.ValidateParameterName(propertyName);
            Set(new ParameterBuilderRenamed(propertyName));
            return this;
        }

        public ParameterBuilderFactory SetDefaultValue(Object value)
        {
            Set(new ParameterBuilderDefaultValue(value));
            return this;
        }

        public ParameterBuilderFactory SetFixedValue(Object value)
        {
            Set(new ParameterBuilderFixed(value));
            return this;
        }

        public ParameterBuilderFactory SynthesizeMandatory(Type parameterType)
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            Synthesize(
                parameterType,
                new AttributeData(typeof(SMA.ParameterAttribute))
                {
                    NamedArguments = {["Mandatory"] = true }
                }
            );
            return this;
        }

        public ParameterBuilderFactory Synthesize(
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

        IChildParameterBuilderFactory IChildParameterBuilderFactory.Ignore()
            => Ignore();

        IChildParameterBuilderFactory IChildParameterBuilderFactory.SetFixedValue(
            Object value
        )
            => SetFixedValue(value);

        private void Set(ParameterBuilder parameter)
        {
            Container.Update(Name, parameter);
        }

        private ParameterBuilderContainer Container { get; }

        private String Name { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderFactory));
    }
}
