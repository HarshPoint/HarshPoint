using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using SMAParameter = System.Management.Automation.ParameterAttribute;

namespace HarshPoint.Shellploy
{
    [Cmdlet(VerbsLifecycle.Invoke, "Provisioner")]
    public sealed class InvokeProvisionerCommand : ClientContextCmdlet
    {
        [SMAParameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public Object InputObject { get; set; }

        [SMAParameter]
        public SwitchParameter Unprovision { get; set; }

        protected override void BeginProcessing()
        {
            Provisioner = new HarshProvisioner();
        }

        protected override void ProcessRecord()
        {
            AddChildren(Provisioner, InputObject);
        }

        protected override void EndProcessing()
        {
            using (var clientContext = CreateClientContext())
            {
                var context = new HarshProvisionerContext(clientContext);
                var cts = new CancellationTokenSource();

                try
                {
                    var sequence = ProvisionerAction(context, cts.Token);

                    foreach (var item in sequence)
                    {
                        if (Stopping)
                        {
                            cts.Cancel();
                            break;
                        }

                        if (item != null)
                        {
                            WriteObject(item);
                        }
                    }
                }
                catch (PipelineStoppedException)
                {
                    cts.Cancel();
                    throw;
                }
                catch (Exception ex)
                {
                    WriteMaybeAggregateException(ex);
                }
            }
        }

        private HarshProvisioner Provisioner { get; set; }

        private IEnumerable<ProgressReport> ProvisionerAction(
            HarshProvisionerContext context,
            CancellationToken token
        )
            => Unprovision ?
                Provisioner.Unprovision(context, token, PollIsStoppingInterval) :
                Provisioner.Provision(context, token, PollIsStoppingInterval);

        private void WriteMaybeAggregateException(Exception exception)
        {
            var exceptions = new[] { exception }.AsEnumerable();

            var aggregate = (exception as AggregateException);
            if (aggregate != null)
            {
                exceptions = aggregate.InnerExceptions;
            }

            foreach (var exc in exceptions)
            {
                WriteError(CreateErrorRecord(exc));
            }
        }

        private static ErrorRecord CreateErrorRecord(Exception exc)
            => new ErrorRecord(
                exception: exc,
                errorId: null,
                errorCategory: ErrorCategory.OperationStopped,
                targetObject: null
            );

        private static readonly TimeSpan PollIsStoppingInterval
            = TimeSpan.FromMilliseconds(250);
    }
}
