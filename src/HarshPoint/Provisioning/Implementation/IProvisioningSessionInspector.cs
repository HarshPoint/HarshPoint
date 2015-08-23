namespace HarshPoint.Provisioning.Implementation
{
    public interface IProvisioningSessionInspector
    {
        void OnSessionStarting(IHarshProvisionerContext context);
        void OnSessionEnded(IHarshProvisionerContext context);
        void OnProvisioningStarting(IHarshProvisionerContext context, HarshProvisionerBase provisioner);
        void OnProvisioningEnded(IHarshProvisionerContext context, HarshProvisionerBase provisioner);
        void OnProvisioningSkipped(IHarshProvisionerContext context, HarshProvisionerBase provisioner);
    }
}
