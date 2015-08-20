using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshFieldRefMetadata :
        HarshPointShellployCommand<HarshFieldRef>
    {
        public HarshFieldRefMetadata()
        {
            Parameter("InternalName").Synthesize(typeof(String));

            Parameter(x => x.Fields).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(Field))
            );
        }
    }
}