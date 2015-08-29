using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildField :
        NewProvisionerCommandBuilder<HarshModifyField>
    {
        public BuildField()
        {
            ProvisionerDefaults.Include(this);

            AsChildOf<HarshField>();
        }
    }
}