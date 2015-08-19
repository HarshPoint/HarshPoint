using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldMultilineTextMetadata :
        HarshPointShellployCommand<HarshModifyFieldMultilineText>
    {
        public HarshModifyFieldMultilineTextMetadata()
        {
            RenameParameter(x => x.Fields, "Field");

            AsChildOf<HarshField>()
                .SetValue(x => x.Type, FieldType.Note);
        }
    }
}