using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshEnableWebFeature : HarshProvisioner
    {
        [Parameter(Mandatory = true)]
        public Guid FeatureId
        {
            get;
            set;
        }

        [Parameter]
        public FeatureDefinitionScope FeatureDefinitionScope
        {
            get;
            set;
        }

        [Parameter]
        public Boolean Force
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            Web.Features.Add(FeatureId, Force, FeatureDefinitionScope);

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
