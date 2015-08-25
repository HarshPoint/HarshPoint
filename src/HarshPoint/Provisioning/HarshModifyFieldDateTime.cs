using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldDateTime : 
        HarshModifyField<FieldDateTime, HarshModifyFieldDateTime>
    {
        public HarshModifyFieldDateTime()
        {
            Map(f => f.DisplayFormat);
            Map(f => f.FriendlyDisplayFormat);
        }
        [Parameter]
        public DateTimeFieldFormatType? DisplayFormat { get; set; }

        [Parameter]
        public DateTimeFieldFriendlyFormatType? FriendlyDisplayFormat { get; set; }
    }
}
