using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldRef :
        NewProvisionerCommandBuilder<HarshFieldRef>
    {
        public BuildFieldRef()
        {
            ProvisionerDefaults.Include(this);

            Parameter("InternalName").Synthesize(typeof(String));

            Parameter(x => x.Fields).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.Field))
                    .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
            );
        }
    }
}