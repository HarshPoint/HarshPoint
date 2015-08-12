using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshListMetadata
        : ShellployMetadataObject<HarshList>
    {
        protected override ShellployCommandBuilder<HarshList> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .AddPositionalParameter(x => x.Title)
                .AddPositionalParameter(x => x.Url)
                .SetDefaultParameterValue(x => x.TemplateType, ListTemplateType.GenericList)
                .HasChildren();
        }
    }
}