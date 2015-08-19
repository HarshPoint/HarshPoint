using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldDateTimeMetadata :
        HarshPointShellployCommand<HarshModifyFieldDateTime>
    {
        public HarshModifyFieldDateTimeMetadata()
        {
            RenameParameter(x => x.Fields, "Field");
            AsChildOf<HarshField>()
               .SetValue(x => x.Type, FieldType.DateTime);
        }
    }
}