using System;
using System.Collections.Generic;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class ParameterBuilder : IChildParameterBuilder
    {
        internal ParameterBuilder(
            PropertyModelContainer container,
            String name
        )
        {
            Container = container;
            Name = name;
        }

        public ParameterBuilder Ignore()
        {
            Update(new PropertyModelIgnored());
            return this;
        }

        public ParameterBuilder Rename(String propertyName)
        {
            CommandBuilder.ValidateParameterName(propertyName);
            Update(new PropertyModelRenamed(propertyName));
            return this;
        }

        public ParameterBuilder SetDefaultValue(Object value)
        {
            Update(new PropertyModelDefaultValue(value));
            return this;
        }

        public ParameterBuilder SetFixedValue(Object value)
        {
            Update(new PropertyModelFixed(value));
            return this;
        }

        public ParameterBuilder SynthesizeMandatory(Type parameterType)
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            Synthesize(
                parameterType,
                new AttributeBuilder(typeof(SMA.ParameterAttribute))
                {
                    Properties =
                    {
                        ["Mandatory"] = true
                    }
                }
            );
            return this;
        }

        public ParameterBuilder Synthesize(
            Type parameterType,
            params AttributeBuilder[] attributes
        )
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            if (attributes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributes));
            }

            Synthesize(parameterType, attributes.Select(a => a.ToModel()));
            return this;
        }

        public ParameterBuilder Synthesize(
            Type parameterType,
            IEnumerable<AttributeModel> attributes
        )
        {
            if (parameterType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterType));
            }

            if (attributes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributes));
            }

            Update(new PropertyModelSynthesized(
                Name,
                parameterType,
                attributes
            ));

            return this;
        }

        IChildParameterBuilder IChildParameterBuilder.Ignore()
            => Ignore();

        IChildParameterBuilder IChildParameterBuilder.SetFixedValue(
            Object value
        )
            => SetFixedValue(value);

        private void Update(PropertyModel parameter)
        {
            Container.Update(Name, parameter);
        }

        private PropertyModelContainer Container { get; }

        private String Name { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilder));
    }
}
