using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldMultilineText : 
        HarshModifyField<FieldMultiLineText, HarshModifyFieldMultilineText>
    {
        public HarshModifyFieldMultilineText()
        {
            Map(f => f.AllowHyperlink);
            Map(f => f.AppendOnly);
            Map(f => f.NumberOfLines);
            Map(f => f.RestrictedMode);
            Map(f => f.RichText);
        }

        [Parameter]
        public Boolean? AllowHyperlink { get; set; }

        [Parameter]
        public Boolean? AppendOnly { get; set; }

        [Parameter]
        public Int32? NumberOfLines { get; set; }

        [Parameter]
        public Boolean? RestrictedMode { get; set; }

        [Parameter]
        public Boolean? RichText { get; set; }
    }
}
