using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldMultilineTextMetadata :
        HarshPointShellployCommand<HarshModifyFieldMultilineText>
    {
        public HarshModifyFieldMultilineTextMetadata()
        {
            AsChildOf<HarshField>(p =>
            {
                p.SetFixedValue(x => x.Type, FieldType.Note);
                p.Ignore(x => x.TypeName);
            });

            Parameter(x => x.Fields).Rename("Field");
        }
    }
}