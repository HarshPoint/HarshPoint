using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldDateTimeMetadata :
        HarshPointShellployCommand<HarshModifyFieldDateTime>
    {
        public HarshModifyFieldDateTimeMetadata()
        {
            AsChildOf<HarshField>(
                parent => parent.SetFixedValue(x => x.Type, FieldType.DateTime)
            );

            Parameter(x => x.Fields).Rename("Field");
        }
    }
}