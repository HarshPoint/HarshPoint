using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.ShellployGenerator
{
    internal abstract class ShellployMetadataObject<TProvisioner> : IShellployMetadataObject
        where TProvisioner : HarshProvisionerBase
    {
        private const String CommandNamespace = "HarshPoint.Shellploy";
        private const String ProvisioningNamespace = "HarshPoint.Provisioning";

        protected virtual ShellployCommandBuilder<TProvisioner> CreateCommandBuilder()
            => new ShellployCommandBuilder<TProvisioner>()
                .InNamespace(CommandNamespace)
                .AddUsing(ProvisioningNamespace);

        public IShellployCommandBuilder GetCommandBuilder() => CreateCommandBuilder();

        public Type GetProvisionerType() => typeof(TProvisioner);
    }
}
