using System;
using System.Collections.Immutable;
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

        public void OnSessionStarting(IHarshProvisionerContext context)
        {
            NotifyInspectors(context, si => si.OnSessionStarting(context));
        }

        public void OnSessionEnded(IHarshProvisionerContext context)
        {
            NotifyInspectors(context, si => si.OnSessionEnded(context));
        }

        public void OnProvisioningStarting(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            NotifyInspectors(context, si => si.OnProvisioningStarting(context, provisioner));
        }

        public void OnProvisioningEnded(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            NotifyInspectors(context, si => si.OnProvisioningEnded(context, provisioner));
        }

        public void OnProvisioningSkipped(IHarshProvisionerContext context, HarshProvisionerBase provisioner)
        {
            foreach (var p in GetFlattenedTree(provisioner))
            {
                NotifyInspectors(context, si => si.OnProvisioningSkipped(context, p));
            }
        }

        public void NotifyInspectors(IHarshProvisionerContext context, Action<IProvisioningSessionInspector> action)
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
