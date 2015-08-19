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
            AddNamedParameter<String>("InternalName");

            SetParameterValue(x => x.Fields,
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(Field))
            );
        }
    }
}