using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldMultilineText : HarshModifyField<FieldMultiLineText>
    {
        public Boolean? AllowHyperlink { get; set; }

        public Boolean? AppendOnly { get; set; }

        public Int32? NumberOfLines { get; set; }

        public Boolean? RestrictedMode { get; set; }

        public Boolean? RichText { get; set; }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                SetPropertyIfHasValue(field, AllowHyperlink, f => f.AllowHyperlink);
                SetPropertyIfHasValue(field, AppendOnly, f => f.AppendOnly);
                SetPropertyIfHasValue(field, NumberOfLines, f => f.NumberOfLines);
                SetPropertyIfHasValue(field, RestrictedMode, f => f.RestrictedMode);
                SetPropertyIfHasValue(field, RichText, f => f.RichText);

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
