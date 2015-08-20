using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldMultilineText :
        HarshPointCommandBuilder<HarshModifyFieldMultilineText>
    {
        public BuildFieldMultilineText()
        {
            AsChildOf<HarshField>(p =>
            {
                p.SetFixedValue(x => x.Type, FieldType.Note);
                p.Ignore(x => x.TypeName);
            });
        }
    }
}