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
    internal sealed class ReactiveOutputSink :
        HarshProvisionerOutputSink,
        IDisposable
    {
        private readonly Subject<HarshProvisionerOutput> _subject
            = new Subject<HarshProvisionerOutput>();

        public ReactiveOutputSink() { }

        public ReactiveOutputSink(CancellationToken token)
        {
            CancellationToken = token;
        }

        public ReactiveOutputSink(TimeSpan? pollInterval)
        {
            PollInterval = pollInterval;
        }

        public ReactiveOutputSink(
            CancellationToken token, 
            TimeSpan? pollInterval
        )
        {
            CancellationToken = token;
            PollInterval = pollInterval;
        }

        public IEnumerable<HarshProvisionerOutput> Provision<TProvisioner, TContext>(
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

        public IEnumerable<HarshProvisionerOutput> Unprovision<TProvisioner, TContext>(
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

        protected override void WriteOutputCore(HarshProvisionerOutput output)
        {
            CancellationToken.ThrowIfCancellationRequested();
            _subject.OnNext(output);
        }

        private IEnumerable<HarshProvisionerOutput> AsEnumerable()
        {
            var observable = _subject.AsObservable();

            if (PollInterval.HasValue)
            {
                var interval = Observable
                    .Interval(PollInterval.Value)
                    .Select(n => (HarshProvisionerOutput)null);

                observable = observable.Merge(interval);
            }

            return observable.Next();
        }

        private IEnumerable<HarshProvisionerOutput> Invoke<TProvisioner, TContext>(
            TProvisioner provisioner,
            TContext context,
            Func<TContext, CancellationToken, Task> action
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            context = context.WithOutputSink(this);

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
            = HarshLog.ForContext(typeof(ReactiveOutputSink));
    }
}
