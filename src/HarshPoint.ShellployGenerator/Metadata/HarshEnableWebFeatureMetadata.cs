using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshEnableWebFeatureMetadata :
        HarshPointShellployCommand<HarshEnableWebFeature>
    {
        public HarshEnableWebFeatureMetadata()
        {
            AddPositionalParameter(x => x.FeatureId);
        }
    }
}