using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HarshPoint.Shellploy
{
    internal static class ProgressReactiveExtensions
    {
        public static IEnumerable<HarshProvisionerRecord> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive();

            return DisposeWhenEnumerated(
                progress,
                progress.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(token);

            return DisposeWhenEnumerated(
                progress,
                progress.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(token, pollInterval);

            return DisposeWhenEnumerated(
                progress,
                progress.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Provision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(pollInterval);

            return DisposeWhenEnumerated(
                progress,
                progress.Provision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive();

            return DisposeWhenEnumerated(
                progress,
                progress.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(token);

            return DisposeWhenEnumerated(
                progress,
                progress.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            CancellationToken token,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(token, pollInterval);

            return DisposeWhenEnumerated(
                progress,
                progress.Unprovision(provisioner, context)
            );
        }

        public static IEnumerable<HarshProvisionerRecord> Unprovision<TProvisioner, TContext>(
            this TProvisioner provisioner,
            TContext context,
            TimeSpan? pollInterval
        )
            where TProvisioner : HarshProvisionerBase<TContext>
            where TContext : HarshProvisionerContextBase<TContext>
        {
            var progress = new ProgressReactive(pollInterval);

            return DisposeWhenEnumerated(
                progress,
                progress.Unprovision(provisioner, context)
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
