using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshEnableWebFeatureMetadata
        : ShellployMetadataObject<HarshEnableWebFeature>
    {
        protected override ShellployCommandBuilder<HarshEnableWebFeature> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .AddPositionalParameter(x => x.FeatureId);
        }
    }
}