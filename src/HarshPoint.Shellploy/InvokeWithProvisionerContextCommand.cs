using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections;
using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using SMA = System.Management.Automation;
using System.Net;

namespace HarshPoint.Shellploy
{
    [CmdletAttribute(VerbsLifecycle.Invoke, "WithProvisionerContext", DefaultParameterSetName = "NoAuth")]
    public sealed class InvokeWithProvisionerContextCommand : ClientContextCmdlet
    {
        [SMA.Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public ScriptBlock ScriptBlock { get; set; }

        protected override void ProcessRecord()
        {
            using (var clientContext = CreateClientContext())
            {
                ScriptBlock.Invoke(new HarshProvisionerContext(clientContext));
            }
        }
    }
}
