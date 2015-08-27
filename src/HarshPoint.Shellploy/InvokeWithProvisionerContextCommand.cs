using System.Management.Automation;
using HarshPoint.Provisioning;
using SMA = System.Management.Automation;
using Serilog;
using System;

namespace HarshPoint.Shellploy
{
    [Cmdlet(VerbsLifecycle.Invoke, "WithProvisionerContext", DefaultParameterSetName = "NoAuth")]
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

        private static Boolean _loggerConfigured;
    }
}
