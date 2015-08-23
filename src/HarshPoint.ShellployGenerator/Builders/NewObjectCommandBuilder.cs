using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using SMA = System.Management.Automation;

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

        protected NewObjectCommandBuilder(
            HarshParameterizedObjectMetadata metadata
        )
            : this(ValidateNotNull(metadata, nameof(metadata)).ObjectType)
        {
            if ((metadata.DefaultParameterSet != null) &&
                (!metadata.DefaultParameterSet.IsImplicit))
            {
                DefaultParameterSetName = metadata.DefaultParameterSet.Name;
            }

            foreach (var grouping in metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                ValidateParameterName(property.Name);

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

                PropertyContainer.Update(property.Name, synthesized);
            }
        }

        internal Type TargetType { get; }

        private static AttributeModel CreateParameterAttribute(Parameter param)
        {
            var attr = new AttributeModel(typeof(SMA.ParameterAttribute));

            if (param.IsMandatory)
            {
                attr = attr.SetProperty("Mandatory", true);
            }

            if (!param.IsCommonParameter)
            {
                attr = attr.SetProperty(
                    "ParameterSetName", 
                    param.ParameterSetName
                );
            }

            return attr;
        }

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
