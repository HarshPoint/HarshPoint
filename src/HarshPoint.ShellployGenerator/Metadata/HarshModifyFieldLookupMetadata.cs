using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldLookupMetadata :
        HarshPointShellployCommand<HarshModifyFieldLookup>
    {
        public HarshModifyFieldLookupMetadata()
        {
            AsChildOf<HarshField>(parent =>
            {
                parent.SetFixedValue(x => x.Type, FieldType.Lookup);
                parent.Ignore(x => x.TypeName);
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

            Parameter(x => x.Fields)
                .Rename("Field");
        }
    }
}