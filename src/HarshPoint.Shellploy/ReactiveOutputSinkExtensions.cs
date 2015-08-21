using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HarshPoint.Shellploy
{
    internal static class ReactiveOutputSinkExtensions
    {
        public static IEnumerable<HarshProvisionerOutput> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink();

            return DisposeWhenEnumerated(
                sink,
                sink.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(token);

            return DisposeWhenEnumerated(
                sink,
                sink.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(token, pollInterval);

            return DisposeWhenEnumerated(
                sink,
                sink.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(pollInterval);

            return DisposeWhenEnumerated(
                sink,
                sink.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink();

            return DisposeWhenEnumerated(
                sink,
                sink.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(token);

            return DisposeWhenEnumerated(
                sink,
                sink.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(token, pollInterval);

            return DisposeWhenEnumerated(
                sink,
                sink.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerOutput> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var sink = new ReactiveOutputSink(pollInterval);

            return DisposeWhenEnumerated(
                sink,
                sink.Unprovision(provisioner, context)
            );
        }

        private static IEnumerable<T> DisposeWhenEnumerated<T>(
            IDisposable disposable,
            IEnumerable<T> sequence
        )
        {
            using (disposable)
            {
                foreach (var item in sequence)
                {
                    yield return item;
                }
            }
        }
    }
}
