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
            AddNamedParameter<String>("TargetListUrl");
            AddNamedParameter<String>("TargetField");
            SetDefaultValue("TargetField", "Title");
            SetParameterValue(x => x.LookupTarget,
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.List))
                    .Call(nameof(Resolve.ByUrl), new CodeVariableReferenceExpression("TargetListUrl"))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("TargetField"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(Tuple<List, Field>))
                );
            RenameParameter(x => x.Fields, "Field");
            AsChildOf<HarshField>()
                .SetValue(x => x.Type, FieldType.Lookup)
                .IgnoreParameter(x => x.TypeName);
        }
    }
}