using Microsoft.SharePoint.Client;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldDateTime : HarshModifyField<FieldDateTime>
    {
        [Parameter]
        public DateTimeFieldFormatType? DisplayFormat
        {
            get;
            set;
        }

        [Parameter]
        public DateTimeFieldFriendlyFormatType? FriendlyDisplayFormat
        {
            get;
            set;
        }

        protected override ClientObjectUpdater GetUpdater() => Updater;

        private static readonly ClientObjectUpdater Updater
            = ClientObjectUpdater.Build<HarshModifyFieldDateTime, FieldDateTime>()
            .Map(f => f.DisplayFormat, p => p.DisplayFormat)
            .Map(f => f.FriendlyDisplayFormat, p => p.FriendlyDisplayFormat)
            .ToClientObjectUpdater();
    }
}
