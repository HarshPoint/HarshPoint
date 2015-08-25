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
        IProgress<ProgressReport>,
        IDisposable
    {
        private readonly Subject<ProgressReport> _subject
            = new Subject<ProgressReport>();

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

        public IEnumerable<ProgressReport> Provision<TProvisioner, TContext>(
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

        public IEnumerable<ProgressReport> Unprovision<TProvisioner, TContext>(
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

        public void Report(ProgressReport report)
        {
            CancellationToken.ThrowIfCancellationRequested();
            _subject.OnNext(report);
        }

        private IEnumerable<ProgressReport> AsEnumerable()
        {
            var observable = _subject.AsObservable();

            if (PollInterval.HasValue)
            {
                var interval = Observable
                    .Interval(PollInterval.Value)
                    .Select(n => (ProgressReport)null);

                observable = observable.MergeWithCompleteOnEither(interval);
            }

            return observable.Next();
        }

        private IEnumerable<ProgressReport> Invoke<TProvisioner, TContext>(
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
