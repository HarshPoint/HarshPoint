using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployMetadata
    {
        private const String CommandNamespace = "HarshPoint.Shellploy";

        private List<IShellployCommandBuilder> builders = new List<IShellployCommandBuilder>();

        public ShellployMetadata()
        {
            Map<HarshContentType>()
                 .PositionalParameters(x => x.Id, x => x.Name)
                 .HasChildren();
            Map<HarshField>()
                 .AsChildOf<HarshContentType>();
        }

        private ShellployCommandBuilder<TProvisioner> Map<TProvisioner>()
            where TProvisioner : HarshProvisionerBase
        {
            var builder = new ShellployCommandBuilder<TProvisioner>();
            builders.Add(builder);
            return builder.InNamespace(CommandNamespace);
        }

        public IEnumerable<ShellployCommand> GetCommands()
        {
            return builders.Select(builder => builder.ToCommand());
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ShellployMetadata>();
    }
}
