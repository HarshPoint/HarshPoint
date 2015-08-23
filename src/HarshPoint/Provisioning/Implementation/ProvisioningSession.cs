using System;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ProvisioningSession
    {
        private readonly IHarshProvisionerContext _context;
        private readonly HarshProvisionerBase _rootProvisioner;
        private readonly HarshProvisionerAction _action;

        private IImmutableList<HarshProvisionerBase> _provisioners;

        public IImmutableList<HarshProvisionerBase> Provisioners
            => HarshLazy.Initialize(ref _provisioners, CreateProvisionersCollection);

        private IImmutableList<HarshProvisionerBase> CreateProvisionersCollection()
            => GetFlattenedTree(_rootProvisioner);

        internal ProvisioningSession(
            IHarshProvisionerContext context,
            HarshProvisionerBase rootProvisioner,
            HarshProvisionerAction action)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (rootProvisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(rootProvisioner));
            }

            _context = context;
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

        public void OnSessionStarting()
        {
            NotifyInspectors(si => si.OnSessionStarting());
        }

        public void OnSessionEnded()
        {
            NotifyInspectors(si => si.OnSessionEnded());
        }

        public void OnProvisioningStarting(HarshProvisionerBase provisioner)
        {
            NotifyInspectors(si => si.OnProvisioningStarting(provisioner));
        }

        public void OnProvisioningEnded(HarshProvisionerBase provisioner)
        {
            NotifyInspectors(si => si.OnProvisioningEnded(provisioner));
        }

        public void OnProvisioningSkipped(HarshProvisionerBase provisioner)
        {
            foreach (var p in GetFlattenedTree(provisioner))
            {
                NotifyInspectors(si => si.OnProvisioningSkipped(p));
            }
        }

        public void NotifyInspectors(Action<IProvisioningSessionInspector> action)
        {
            foreach (var sessionInspector in _context.SessionInspectors)
            {
                action(sessionInspector);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisioningSession));
    }

}
