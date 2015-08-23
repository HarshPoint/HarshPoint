namespace HarshPoint.Provisioning.Implementation
{
    public interface IProvisioningSessionInspector
    {
        void OnSessionStarting();
        void OnSessionEnded();
        void OnProvisioningStarting(HarshProvisionerBase provisioner);
        void OnProvisioningEnded(HarshProvisionerBase provisioner);
        void OnProvisioningSkipped(HarshProvisionerBase provisioner);
    }
}
