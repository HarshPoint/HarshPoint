using HarshPoint.Provisioning.Implementation;
using HarshPoint.ShellployGenerator.Builders;
using System.Text.RegularExpressions;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal abstract class HarshPointCommandBuilder<T> : CommandBuilder<T>
        where T : HarshProvisionerBase
    {
        public HarshPointCommandBuilder()
        {
            Aliases.Add(Regex.Replace(typeof(T).Name, "^Harsh(Modify)?", ""));

            Namespace = "HarshPoint.Shellploy";
            ImportedNamespaces.Add("HarshPoint.Provisioning");

            Parameter("Fields").Rename("Field");
            Parameter("Lists").Rename("List");
        }
    }
}
