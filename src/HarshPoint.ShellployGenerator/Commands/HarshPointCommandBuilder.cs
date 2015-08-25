using HarshPoint.Provisioning.Implementation;
using HarshPoint.ShellployGenerator.Builders;
using System.Text.RegularExpressions;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal abstract class HarshPointCommandBuilder<T> : NewProvisionerCommandBuilder<T>
        where T : HarshProvisionerBase
    {
        public HarshPointCommandBuilder()
        {
            Noun = Regex.Replace(typeof(T).Name, "^Harsh(Modify)?", "");
            Aliases.Add(Noun);

            Namespace = "HarshPoint.Shellploy";
            ImportedNamespaces.Add("HarshPoint.Provisioning");

            Parameter("Fields").Rename("Field");
            Parameter("Lists").Rename("List");
        }
    }
}
