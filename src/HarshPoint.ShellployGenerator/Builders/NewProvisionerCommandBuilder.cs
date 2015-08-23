using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class NewProvisionerCommandBuilder :
        NewObjectCommandBuilder
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

        protected virtual IEnumerable<PropertyModel> CreatePropertiesLocal()
            => BoolToSwitchVisitor.Visit(
                SetValueFromPipelineByPropertyName.Visit(
                    PropertyContainer
                )
            );

        internal IChildCommandBuilder ChildBuilder { get; set; }

        private IEnumerable<PropertyModel> CreatePropertiesRecursively()
        {
            var localProperties = CreatePropertiesLocal();

            if (ChildBuilder != null)
            {
                var parentProperties = ChildBuilder.ParameterBuilders.ApplyTo(
                    ParentBuilder.CreatePropertiesRecursively()
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
                    var result= Context.GetNewObjectCommandBuilder(
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

        private static readonly PropertyModelVisitor SetValueFromPipelineByPropertyName
            = new AttributeNamedArgumentVisitor(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipelineByPropertyName",
                true
            );


        private const String HarshProvisionerCmdlet = "HarshProvisionerCmdlet";
    }
}
