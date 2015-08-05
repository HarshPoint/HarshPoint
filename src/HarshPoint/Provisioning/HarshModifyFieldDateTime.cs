using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshModifyFieldDateTime : HarshModifyField<FieldDateTime>
    {
        public DateTimeFieldFormatType? DisplayFormat
        {
            get;
            set;
        }

        public DateTimeFieldFriendlyFormatType? FriendlyDisplayFormat
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                if (DisplayFormat.HasValue)
                {
                    field.DisplayFormat = DisplayFormat.Value;
                }

                if (FriendlyDisplayFormat.HasValue)
                {
                    field.FriendlyDisplayFormat = FriendlyDisplayFormat.Value;
                }

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
