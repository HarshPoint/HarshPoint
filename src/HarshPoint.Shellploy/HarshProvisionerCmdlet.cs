using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Shellploy
{
    public abstract class HarshProvisionerCmdlet<TProvisioner, TContext> : PSCmdlet
        where TProvisioner : HarshProvisionerBase<TContext>
        where TContext : HarshProvisionerContextBase
    {
        protected static void ProcessChildren(TProvisioner provisioner, ScriptBlock children)
        {
            if (children != null)
            {
                var psChildren = children.Invoke()
                    .Select (c => c.BaseObject);
                foreach (var child in psChildren)
                {
                    HarshProvisionerTreeBuilder.AddChild(provisioner, child);
                }
            }
        }
    }
}
