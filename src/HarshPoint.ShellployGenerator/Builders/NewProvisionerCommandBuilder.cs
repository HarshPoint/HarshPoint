using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class NewProvisionerCommandBuilder : NewObjectCommandBuilder
    {
        protected NewProvisionerCommandBuilder(
            HarshProvisionerMetadata metadata
        )
            : base(metadata)
        {
            BaseTypes.Remove(typeof(SMA.PSCmdlet).FullName);
            BaseTypes.Add(HarshProvisionerCmdlet);
        }

        protected sealed override IEnumerable<PropertyModel> CreateProperties()
            => CreatePropertiesRecursively();

        protected internal override void ValidatePropertyName(String name)
        {
            base.ValidatePropertyName(name);

            if (InputObjectName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(name),
                    SR.CommandBuilder_ReservedName,
                    name
                );
            }
        }

        protected virtual IEnumerable<PropertyModel> CreatePropertiesLocal()
        {
            var properties = base.CreateProperties();

            properties = IgnoreUnfixedParameterSets.Visit(properties);
            properties = RemoveIgnoredUnsynthesized.Visit(properties);
            properties = SetValueFromPipelineByPropertyName.Visit(properties);
            properties = BoolToSwitchVisitor.Visit(properties);

            if (HasInputObject)
            {
                properties = properties.Concat(
                    InputObjectProperty
                );
            }

            return properties;
        }

        internal IChildProvisionerCommandBuilder ChildBuilder { get; set; }

        private IEnumerable<PropertyModel> CreatePropertiesRecursively()
        {
            var localProperties = CreatePropertiesLocal();

            if (ChildBuilder != null)
            {
                var parentProperties = ChildBuilder.PropertyContainer.ApplyTo(
                    ParentBuilder.CreatePropertiesRecursively()
                );

                // need to call this here again to apply any fixed values 
                // specified by ChildBuilder.

                parentProperties = IgnoreUnfixedParameterSets.Visit(
                    parentProperties
                );

                // remove any ignored properties right away, don't want them
                // to get processed by any children

                parentProperties = RemoveIgnoredUnsynthesized.Visit(
                    parentProperties
                );

                return parentProperties.Concat(localProperties);
            }

            return localProperties;
        }

        private NewProvisionerCommandBuilder ParentBuilder
        {
            get
            {
                if (ChildBuilder != null)
                {
                    var result = Context.GetNewObjectCommandBuilder(
                        ChildBuilder.ParentType
                    );

                    return (NewProvisionerCommandBuilder)result;
                }

                return null;
            }
        }

        private IImmutableList<Type> ParentTargetTypes
        {
            get
            {
                if (ParentBuilder != null)
                {
                    return ParentBuilder
                        .ParentTargetTypes
                        .Add(ParentBuilder.TargetType);
                }

                return ImmutableList<Type>.Empty;
            }
        }

        private static readonly ChangePropertyTypeVisitor BoolToSwitchVisitor =
            new ChangePropertyTypeVisitor(
                typeof(Boolean),
                typeof(SMA.SwitchParameter)
            );

        private static readonly PropertyModelVisitor IgnoreUnfixedParameterSets
            = new IgnoreUnfixedParameterSetPropertiesVisitor();

        private static readonly PropertyModelVisitor SetValueFromPipelineByPropertyName
            = new AttributeNamedArgumentVisitor(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipelineByPropertyName",
                true
            );

        private static readonly AttributeModel ValueFromPipeline
            = new AttributeModel(typeof(SMA.ParameterAttribute))
                .SetProperty("ValueFromPipeline", true);

        private static readonly PropertyModel InputObjectProperty
            = new PropertyModelPositional(
                Int32.MaxValue,
                new PropertyModelInputObject(
                    new PropertyModelSynthesized(
                        InputObjectName,
                        typeof(Object),
                        ValueFromPipeline
                    )
                )
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewProvisionerCommandBuilder));

        internal const String InputObjectName = "InputObject";

        private const String HarshProvisionerCmdlet = "HarshProvisionerCmdlet";
    }
}
