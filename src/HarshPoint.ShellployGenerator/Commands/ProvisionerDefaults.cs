using HarshPoint.Provisioning.Implementation;
using HarshPoint.ShellployGenerator.Builders;
using System.Text.RegularExpressions;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal static class ProvisionerDefaults
    {
        public static void Include<T>(
            NewProvisionerCommandBuilder<T> builder
        )
            where T : HarshProvisionerBase
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            builder.Noun = Regex.Replace(typeof(T).Name, "^Harsh(Modify)?", "");
            builder.Aliases.Add(builder.Noun);

            builder.Namespace = "HarshPoint.Shellploy";
            builder.ImportedNamespaces.Add("HarshPoint.Provisioning");

            builder.Parameter("Fields").Rename("Field");
            builder.Parameter("Lists").Rename("List");
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisionerDefaults));
    }
}
