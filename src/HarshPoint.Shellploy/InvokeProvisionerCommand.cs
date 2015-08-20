using System;
using System.Management.Automation;
using HarshPoint.Provisioning;
using SMA = System.Management.Automation;

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
                    //.WithOutputSink(new PowerShellOutputSink(this));

                var provisioner = new HarshProvisioner();
                AddChildren(provisioner, Children);

                try
                {
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
