using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldUser :
        NewProvisionerCommandBuilder<HarshModifyFieldUser>
    {
        public BuildFieldUser()
        {
            ProvisionerDefaults.Include(this);

            AsChildOf<HarshField>(
                p => p.Parameter(x => x.Type).SetFixedValue(FieldType.User)
            );

            PositionalParameter("SelectionGroupId").SynthesizeMandatory(
                typeof(Int32[])
            );

            Parameter(x => x.SelectionGroup).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.Group))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("SelectionGroupId"))
            );
        }
    }
}