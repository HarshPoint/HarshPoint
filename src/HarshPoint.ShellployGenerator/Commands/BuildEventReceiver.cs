using HarshPoint.Provisioning;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildEventReceiver :
        HarshPointCommandBuilder<HarshEventReceiver>
    {
        public BuildEventReceiver()
        {
            Parameter("ListUrl").SynthesizeMandatory(typeof(String[]));

            Parameter(x => x.Lists)
                .SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.List))
                    .Call(nameof(Resolve.ByUrl), new CodeVariableReferenceExpression("ListUrl"))
            );

        }
    }
}
