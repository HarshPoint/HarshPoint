using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Provisioning
{
    public sealed class HarshProvisionerCollection<T> : Collection<T>
        where T : HarshProvisionerBase
    {
        internal HarshProvisionerCollection(Func<T, HarshProvisionerBase> prepareProvisioner)
        {
            if (prepareProvisioner == null)
            {
                throw Error.ArgumentNull("prepareProvisioner");
            }

            PrepareProvisioner = prepareProvisioner;
        }

        internal void Provision()
        {
            foreach (var provisioner in this.Select(PrepareProvisioner))
            {
                provisioner.Provision();
            }
        }

        internal void Unprovision()
        {
            foreach (var provisioner in this.Reverse().Select(PrepareProvisioner))
            {
                provisioner.Unprovision();
            }
        }

        internal Func<T, HarshProvisionerBase> PrepareProvisioner
        {
            get;
            private set;
        }
    }
}
