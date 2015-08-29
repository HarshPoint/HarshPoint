using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildEnableWebFeature :
        NewProvisionerCommandBuilder<HarshEnableWebFeature>
    {
        public BuildEnableWebFeature()
        {
            ProvisionerDefaults.Include(this);

            PositionalParameter(x => x.FeatureId);
        }
    }
}