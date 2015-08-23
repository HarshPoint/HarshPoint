using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class NewProvisionerCommandBuilder<TProvisioner> :
        NewObjectCommandBuilder<TProvisioner>
        where TProvisioner : HarshProvisionerBase
    {
        public NewProvisionerCommandBuilder()
            : base(Metadata)
        {
            BaseTypes.Remove(typeof(SMA.PSCmdlet).FullName);
            BaseTypes.Add(HarshProvisionerCmdlet);
        }

        protected override IEnumerable<PropertyModel> CreatePropertiesLocal()
            => BoolToSwitchVisitor.Visit(
                base.CreatePropertiesLocal()
            );

        private const String HarshProvisionerCmdlet = "HarshProvisionerCmdlet";

        private static readonly ChangePropertyTypeVisitor BoolToSwitchVisitor =
            new ChangePropertyTypeVisitor(
                typeof(Boolean),
                typeof(SMA.SwitchParameter)
            );

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewProvisionerCommandBuilder<>));
    }
}