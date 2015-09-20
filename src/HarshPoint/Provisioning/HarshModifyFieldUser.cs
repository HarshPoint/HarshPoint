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
            Map(f => f.SelectionGroup).From(
                p => p.SelectionGroup?.Value?.Id ?? -1
            );
        }

        [Parameter]
        public FieldUserSelectionMode SelectionMode { get; set; }

        [Parameter]
        public IResolveSingle<Group> SelectionGroup { get; set; }
    }
}
