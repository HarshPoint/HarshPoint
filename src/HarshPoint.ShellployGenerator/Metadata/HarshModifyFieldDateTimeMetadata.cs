using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldDateTimeMetadata
        : ShellployMetadataObject<HarshModifyFieldDateTime>
    {
        protected override ShellployCommandBuilder<HarshModifyFieldDateTime> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .RenameParameter(x => x.Fields, "Field")
                .AsChildOf<HarshField>()
                    .SetValue(x => x.Type, FieldType.DateTime)
                .End();
        }
    }
}