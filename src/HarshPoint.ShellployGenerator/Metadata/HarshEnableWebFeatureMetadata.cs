using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshEnableWebFeatureMetadata :
        HarshPointShellployCommand<HarshEnableWebFeature>
    {
        public HarshEnableWebFeatureMetadata()
        {
            PositionalParameter(x => x.FeatureId);
        }
    }
}