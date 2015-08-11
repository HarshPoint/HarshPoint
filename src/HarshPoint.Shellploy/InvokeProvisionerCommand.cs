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
    [CmdletAttribute(VerbsLifecycle.Invoke, "Provisioner")]
    public sealed class InvokeProvisionerCommand : ClientContextCmdlet
    {
        [SMA.Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public ScriptBlock Children { get; set; }

        protected override void ProcessRecord()
        {
            using (var clientContext = CreateClientContext())
            {
                var context = new HarshProvisionerContext(clientContext);
                try
                {
                    var provisioner = new HarshProvisioner();
                    HarshProvisionerTreeBuilder.AddChildren(provisioner, Children);

                    provisioner.ProvisionAsync(context).Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var iex in ex.InnerExceptions)
                    {
                        WriteError(new ErrorRecord(iex, null, ErrorCategory.OperationStopped, null));
                    }
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, null, ErrorCategory.OperationStopped, null));
                }
            }
        }
    }
}
