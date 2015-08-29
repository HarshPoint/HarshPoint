using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildEventReceiver :
        NewProvisionerCommandBuilder<HarshEventReceiver>
    {
        public BuildEventReceiver()
        {
            ProvisionerDefaults.Include(this);

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
