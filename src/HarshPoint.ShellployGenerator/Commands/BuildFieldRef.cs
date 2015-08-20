using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldRef :
        HarshPointCommandBuilder<HarshFieldRef>
    {
        public BuildFieldRef()
        {
            Parameter("InternalName").Synthesize(typeof(String));

            Parameter(x => x.Fields).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
            );
        }
    }
}