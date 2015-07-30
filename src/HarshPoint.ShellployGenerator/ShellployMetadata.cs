using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
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

        private Dictionary<Type, IShellployCommandBuilder> builders
            = new Dictionary<Type, IShellployCommandBuilder>();

        public ShellployMetadata()
        {
            Map<HarshContentType>()
                 .PositionalParameters(x => x.Id, x => x.Name)
                 .HasChildren();

            Map<HarshField>();

            Map<HarshDateTimeField>()
                .AsChildOf<HarshField>()
                    .AddFixedParameter(x => x.Type, FieldType.DateTime);

            Map<HarshFieldMultilineText>()
                .AsChildOf<HarshField>()
                    .AddFixedParameter(x => x.Type, FieldType.Note);
        }

        private ShellployCommandBuilder<TProvisioner> Map<TProvisioner>()
            where TProvisioner : HarshProvisionerBase
        {
            var builder = new ShellployCommandBuilder<TProvisioner>();
            builders.Add(typeof(TProvisioner), builder);
            return builder.InNamespace(CommandNamespace);
        }

        public IEnumerable<ShellployCommand> GetCommands()
        {
            return builders.Values.Select(builder => builder.ToCommand(builders));
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ShellployMetadata>();
    }
}
