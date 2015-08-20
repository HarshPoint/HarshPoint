using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.ShellployGenerator
{
    internal abstract class HarshPointShellployCommand<T> : CommandBuilder<T>
        where T : HarshProvisionerBase
    {
        public HarshPointShellployCommand()
        {
            Namespace = "HarshPoint.Shellploy";
            ImportedNamespaces.Add("HarshPoint.Provisioning");
        }
    }
}
