using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildEnableWebFeature :
        HarshPointCommandBuilder<HarshEnableWebFeature>
    {
        public BuildEnableWebFeature()
        {
            PositionalParameter(x => x.FeatureId);
        }
    }
}