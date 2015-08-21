using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class NewProvisionerCommandBuilder<TProvisioner> :
        NewObjectCommandBuilder<TProvisioner>
        where TProvisioner : HarshProvisionerBase
    {
        public NewProvisionerCommandBuilder()
            : base(Metadata)
        {
        }

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewProvisionerCommandBuilder<>));
    }
}
