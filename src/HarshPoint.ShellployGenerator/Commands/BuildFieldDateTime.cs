using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldDateTime :
        HarshPointCommandBuilder<HarshModifyFieldDateTime>
    {
        public BuildFieldDateTime()
        {
            AsChildOf<HarshField>(
                p => p.Parameter(x => x.Type).SetFixedValue(FieldType.DateTime)
            );
        }
    }
}