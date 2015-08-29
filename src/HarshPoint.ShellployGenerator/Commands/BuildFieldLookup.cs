using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldLookup :
        NewProvisionerCommandBuilder<HarshModifyFieldLookup>
    {
        public BuildFieldLookup()
        {
            ProvisionerDefaults.Include(this);
            
            AsChildOf<HarshField>(parent =>
            {
                parent.Parameter(x => x.Type)
                    .SetFixedValue(FieldType.Lookup);
            });

            Parameter("TargetListUrl")
                .Synthesize(typeof(String));

            Parameter("TargetField")
                .SetDefaultValue("Title")
                .Synthesize(typeof(String));

            Parameter(x => x.LookupTarget)
                .SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.List))
                    .Call(nameof(Resolve.ByUrl), new CodeVariableReferenceExpression("TargetListUrl"))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("TargetField"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(Tuple<List, Field>))
            );
        }
    }
}