using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal interface IProvisioningSessionInspector<TContext>
        where TContext : HarshProvisionerContextBase<TContext>
    {
        void OnSessionStarting();
        void OnSessionEnded();
        void OnProvisioningStarting(HarshProvisionerBase<TContext> provisioner);
        void OnProvisioningEnded(HarshProvisionerBase<TContext> provisioner);
    }
}
