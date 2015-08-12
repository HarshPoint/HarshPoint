using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal abstract class ShellployMetadataObject<TProvisioner> : IShellployMetadataObject
        where TProvisioner : HarshProvisionerBase
    {
        private const String CommandNamespace = "HarshPoint.Shellploy";
        private const String ProvisioningNamespace = "HarshPoint.Provisioning";

        protected virtual ShellployCommandBuilder<TProvisioner> CreateCommandBuilder()
        {
            return new ShellployCommandBuilder<TProvisioner>()
                .InNamespace(CommandNamespace)
                .AddUsing(ProvisioningNamespace);
        }

        public IShellployCommandBuilder GetCommandBuilder()
        {
            return CreateCommandBuilder();
        }

        public Type GetProvisionerType()
        {
            return typeof(TProvisioner);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ShellployMetadataObject<>));
    }
}
