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
                parent => parent.SetFixedValue(x => x.Type, FieldType.DateTime)
            );
        }
    }
}