using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ProvisioningSession
    {
        private readonly HarshProvisionerBase _rootProvisioner;
        private readonly HarshProvisionerAction _action;

        private IImmutableList<HarshProvisionerBase> _provisioners;

        public IImmutableList<HarshProvisionerBase> Provisioners
            => HarshLazy.Initialize(ref _provisioners, CreateProvisionersCollection);

        private IImmutableList<HarshProvisionerBase> CreateProvisionersCollection()
            => GetFlattenedTree(_rootProvisioner);

        internal ProvisioningSession(
            HarshProvisionerBase rootProvisioner,
            HarshProvisionerAction action)
        {
            if (rootProvisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(rootProvisioner));
            }

            _rootProvisioner = rootProvisioner;
            _action = action;
        }

        private IImmutableList<HarshProvisionerBase> GetFlattenedTree(
            HarshProvisionerBase provisioner
        )
        {
            var children = from child in provisioner.GetChildrenSorted(_action)
                           from childRec in GetFlattenedTree(child)
                           select childRec;

            return ImmutableList
                .Create(provisioner)
                .AddRange(children);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnSessionStarting(IHarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnSessionStarting(context));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnSessionEnded(IHarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnSessionEnded(context));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnProvisioningStarting(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnProvisioningStarting(context, provisioner));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnProvisioningEnded(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnProvisioningEnded(context, provisioner));
        }

        public void OnProvisioningSkipped(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            foreach (var p in GetFlattenedTree(provisioner))
            {
                NotifyInspectors(context, si => si.OnProvisioningSkipped(context, p));
            }
        }

        private static void NotifyInspectors(IHarshProvisionerContext context, Action<IProvisioningSessionInspector> action)
        {
            foreach (var sessionInspector in context.SessionInspectors)
            {
                action(sessionInspector);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisioningSession));
    }

}
