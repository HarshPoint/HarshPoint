using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using SMAParameter = System.Management.Automation.ParameterAttribute;
using static HarshPoint.HarshFormattable;
using System.Globalization;

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
                var progressInspector = new SessionProgressInspector();
                var context = new HarshProvisionerContext(clientContext)
                    .AddSessionInspector(progressInspector);
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

                        WriteProgress(progressInspector.Current);
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
                finally
                {
                    WriteProgress(progressInspector.Current, completed: true);
                }
            }
        }

        private HarshProvisioner Provisioner { get; set; }

        private IEnumerable<HarshProvisionerRecord> ProvisionerAction(
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

        private void WriteProgress(
            SessionProgress progress,
            Boolean completed = false
        )
        {
            if (progress == null)
            {
                return;
            }

            String activity;
            if (progress.Action == HarshProvisionerAction.Provision)
            {
                activity = SR.ProvisioningProgressAction;
            }
            else
            {
                activity = SR.UnprovisioningProgressAction;
            }

            var progressRecord = new ProgressRecord(
                ProgressActivityId,
                activity,
                String.Format(
                    CultureInfo.CurrentCulture,
                    SR.ProgressStatusMessage,
                    progress.CompletedProvisionersCount,
                    progress.ProvisionersCount
                 )
            )
            {
                PercentComplete = progress.PercentComplete,
            };

            if (progress.RemainingTime.HasValue)
            {
                progressRecord.SecondsRemaining
                    = (Int32)progress.RemainingTime.Value.TotalSeconds;
            }

            if (progress.CurrentProvisioner != null)
            {
                var currentOperation = progress.CurrentProvisioner.ToString();
                if (progress.CurrentProvisionerIsSkipped)
                {
                    currentOperation = String.Format(
                        CultureInfo.CurrentCulture,
                        SR.ProgressOperationSkipped,
                        currentOperation
                    );
                }
                progressRecord.CurrentOperation = currentOperation;
            }

            if (completed)
            {
                progressRecord.RecordType = ProgressRecordType.Completed;
            }

            WriteProgress(progressRecord);
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

        private const Int32 ProgressActivityId = 0;
    }
}
