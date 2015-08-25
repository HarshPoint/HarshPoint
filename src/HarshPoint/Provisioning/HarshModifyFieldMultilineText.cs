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

        public Boolean? AllowHyperlink { get; set; }

        public Boolean? AppendOnly { get; set; }

        public Int32? NumberOfLines { get; set; }

        public Boolean? RestrictedMode { get; set; }

        public Boolean? RichText { get; set; }
    }
}
