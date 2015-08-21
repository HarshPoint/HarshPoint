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
                p.Parameter(x => x.Type).SetFixedValue(FieldType.Note);
                p.Parameter(x => x.TypeName).Ignore();
            });
        }
    }
}