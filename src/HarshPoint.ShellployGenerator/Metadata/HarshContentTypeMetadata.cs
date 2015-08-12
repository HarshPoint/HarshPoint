using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshContentTypeMetadata
        : ShellployMetadataObject<HarshContentType>
    {
        protected override ShellployCommandBuilder<HarshContentType> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .AddPositionalParameter(x => x.Id)
                .AddPositionalParameter(x => x.Name)
                .HasChildren();
        }
    }
}