using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ProvisioningSession<TContext>
        where TContext : HarshProvisionerContextBase<TContext>
    {
        private readonly HarshProvisionerBase<TContext> _rootProvisioner;

        private IImmutableList<HarshProvisionerBase<TContext>> _provisioners;

        internal ProvisioningSession()
        {
        }

        public IImmutableList<HarshProvisionerBase<TContext>> Provisioners
        {
            get
            {
                if (_provisioners == null)
                {
                    _provisioners = _rootProvisioner.GetProvisionersInOrder();
                }

                return _provisioners;
            }
        }

        public ProvisioningSession(HarshProvisionerBase<TContext> rootProvisioner)
        {
            if (rootProvisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(rootProvisioner));
            }

            _rootProvisioner = rootProvisioner;
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisioningSession<>));
    }
}
