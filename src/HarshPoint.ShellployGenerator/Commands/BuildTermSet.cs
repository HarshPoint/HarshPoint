using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildTermSet :
        NewProvisionerCommandBuilder<HarshTermSet>
    {
        public BuildTermSet()
        {
            ProvisionerDefaults.Include(this);
            HasInputObject = true;
        }
    }
}
