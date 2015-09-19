using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldUser :
        HarshModifyField<FieldUser, HarshModifyFieldUser>
    {
        public HarshModifyFieldUser()
        {
            Map(f => f.SelectionMode);
            Map(f => f.SelectionGroup);
        }

        [Parameter]
        public FieldUserSelectionMode SelectionMode { get; set; }

        [Parameter]
        public Int32 SelectionGroup { get; set; }
    }
}
