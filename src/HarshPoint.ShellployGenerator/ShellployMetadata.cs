using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator
{
    internal static class ShellployMetadata
    {
        private const String CommandNamespace = "HarshPoint.Shellploy";
        private const String ProvisioningNamespace = "HarshPoint.Provisioning";

        public static IEnumerable<ShellployCommand> GetCommands()
        {
            var baseType = typeof(IShellployMetadataObject);
            var builders = baseType.Assembly
                .DefinedTypes
                .Where(type =>
                    baseType.IsAssignableFrom(type)
                    && !type.IsAbstract
                )

                .Select(type => (IShellployMetadataObject)Activator.CreateInstance(type))
                .ToImmutableDictionary(
                    metadata => metadata.GetProvisionerType(),
                    metadata => metadata.GetCommandBuilder()
                );

            return builders.Values.Select(builder => builder.ToCommand(builders));
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ShellployMetadata));
    }
}
