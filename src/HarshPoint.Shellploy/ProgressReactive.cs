using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace HarshPoint.Shellploy
{
    internal sealed class ProgressReactive :
        IProgress<HarshProvisionerRecord>,
        IDisposable
    {
        private readonly Subject<HarshProvisionerRecord> _subject
            = new Subject<HarshProvisionerRecord>();

        public ProgressReactive() { }

        public ProgressReactive(CancellationToken token)
        {
            CancellationToken = token;
        }

        public ProgressReactive(TimeSpan? pollInterval)
        {
            PollInterval = pollInterval;
        }

        public ProgressReactive(
            CancellationToken token,
            TimeSpan? pollInterval
        )
        {
            CancellationToken = token;
            PollInterval = pollInterval;
        }

        public IEnumerable<HarshProvisionerRecord> Provision<TProvisioner, TContext>(
            TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Invoke(provisioner, context, provisioner.ProvisionAsync);
        }

        public IEnumerable<HarshProvisionerRecord> Unprovision<TProvisioner, TContext>(
            TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return Invoke(provisioner, context, provisioner.UnprovisionAsync);
        }

        public void Dispose()
        {
            _subject.Dispose();
        }

        public void Report(HarshProvisionerRecord report)
        {
            CancellationToken.ThrowIfCancellationRequested();
            _subject.OnNext(report);
        }

        private IEnumerable<HarshProvisionerRecord> AsEnumerable()
        {
            var observable = _subject.AsObservable();

            if (PollInterval.HasValue)
            {
                var interval = Observable
                    .Interval(PollInterval.Value)
                    .Select(n => (HarshProvisionerRecord)null);

                observable = observable.MergeWithCompleteOnEither(interval);
            }

            return observable.Next();
        }

        private IEnumerable<HarshProvisionerRecord> Invoke<TProvisioner, TContext>(
            TProvisioner provisioner,
            TContext context,
            Func<TContext, CancellationToken, Task> action
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            context = context.WithProgress(this);

            action(context, CancellationToken).ContinueWith(
                OnProvisioningComplete
            );

            return AsEnumerable();
        }

        private void OnProvisioningComplete(Task t)
        {
            if (t.IsFaulted)
            {
                _subject.OnError(t.Exception);
            }
            else if (!t.IsCanceled)
            {
                _subject.OnCompleted();
            }
        }

        private CancellationToken CancellationToken { get; }

        private TimeSpan? PollInterval { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProgressReactive));
    }
}
