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
            Map(f => f.SelectionGroup)
                .From(p => p.SelectionGroup.Value.Id)
                .When(p => p.SelectionGroup != null);
        }

        [Parameter]
        public FieldUserSelectionMode SelectionMode { get; set; }

        [Parameter]
        public IResolveSingle<Group> SelectionGroup { get; set; }
    }
}
