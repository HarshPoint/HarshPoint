using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildTermGroup : 
        NewProvisionerCommandBuilder<HarshTermGroup>
    {
        public BuildTermGroup()
        {
            ProvisionerDefaults.Include(this);
            HasInputObject = true;
        }
    }
}
