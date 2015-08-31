using HarshPoint.ObjectModel;
using System;
using System.Linq;
using SMA = System.Management.Automation;
using HarshPoint.ShellployGenerator.CodeGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class NewObjectCommandBuilder : CommandBuilder
    {
        protected NewObjectCommandBuilder(Type targetType)
        {
            if (targetType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetType));
            }

            Attributes.Add(new AttributeBuilder(typeof(SMA.OutputTypeAttribute))
            {
                Arguments = { targetType }
            });

            Noun = targetType.Name;
            Verb = SMA.VerbsCommon.New;

            TargetType = targetType;
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected NewObjectCommandBuilder(
            HarshParameterizedObjectMetadata metadata
        )
            : this(ValidateNotNull(metadata, nameof(metadata)).ObjectType)
        {
            if (metadata == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(metadata));
            }

            Metadata = metadata;

            if ((metadata.DefaultParameterSet != null) &&
                (!metadata.DefaultParameterSet.IsImplicit))
            {
                DefaultParameterSetName = metadata.DefaultParameterSet.Name;
            }

            foreach (var grouping in metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                ValidatePropertyName(property.Name);

                if (parameters.Any(p => p.IsCommonParameter))
                {
                    parameters = parameters.Take(1);
                }

                var attributes = parameters.Select(CreateParameterAttribute);

                var synthesized = new PropertyModelSynthesized(
                    property.Name,
                    property.PropertyType,
                    attributes
                );

                var assigned = new PropertyModelAssignedTo(
                    property.PropertyInfo,
                    synthesized
                );

                PropertyContainer.Update(property.Name, assigned);
            }
        }

        protected virtual AttributeModel CreateParameterAttribute(
            Parameter parameter
        )
        {
            if (parameter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameter));
            }

            var attr = new AttributeModel(typeof(SMA.ParameterAttribute));

            if (parameter.IsMandatory)
            {
                attr = attr.SetProperty("Mandatory", true);
            }

            if (!parameter.IsCommonParameter)
            {
                attr = attr.SetProperty(
                    "ParameterSetName",
                    parameter.ParameterSetName
                );
            }

            return attr;
        }

        public NewObjectCommandModel ToNewObjectCommand()
            => ToNewObjectCommand(null);

        public virtual NewObjectCommandModel ToNewObjectCommand(
            IEnumerable<PropertyModel> properties
        ) 
            => new NewObjectCommandModel(
                ToCommand(properties),
                TargetType
            );

        public override CommandCodeGenerator ToCodeGenerator()
        {
            var newObjectCodeGen = new NewObjectCommandCodeGenerator(
                ToNewObjectCommand()
            );

            return newObjectCodeGen.ToCommandCodeGenerator();
        }

        internal HarshObjectMetadata Metadata { get; }

        internal Type TargetType { get; }

        private static T ValidateNotNull<T>(T value, String paramName)
            where T : class
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(paramName);
            }

            return value;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectCommandBuilder));
    }
}
