using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ProvisioningSession
    {
        private readonly HarshProvisionerBase _rootProvisioner;
        private readonly HarshProvisionerAction _action;

        private IImmutableList<HarshProvisionerBase> _provisioners;

        internal ProvisioningSession() { }

        public IImmutableList<HarshProvisionerBase> Provisioners
            => HarshLazy.Initialize(ref _provisioners, CreateProvisionersCollection);

        private IImmutableList<HarshProvisionerBase> CreateProvisionersCollection()
            => _rootProvisioner.GetProvisionersInOrder(_action);
        public ProvisioningSession(
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


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisioningSession));
    }

}
