using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IProvisioningSession
    {
        HarshProvisionerAction Action { get; }
        IImmutableList<HarshProvisionerBase> Provisioners { get; }

        void OnSessionStarting(IHarshProvisionerContext context);
        void OnSessionEnded(IHarshProvisionerContext context);
        void OnProvisioningStarting(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        );
        void OnProvisioningEnded(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        );
        void OnProvisioningSkipped(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        );
    }
}