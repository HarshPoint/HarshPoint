using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using SMA = System.Management.Automation;

namespace HarshPoint.Shellploy
{
    public abstract class ClientContextCmdlet : HarshProvisionerCmdlet
    {
        [SMA.Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public Uri Url { get; set; }

        [SMA.Parameter(ParameterSetName = "DefaultCredential", ValueFromPipelineByPropertyName = true)]
        public CredentialType CredentialType { get; set; } = CredentialType.Default;

        [SMA.Parameter(ParameterSetName = "DefaultCredential", ValueFromPipelineByPropertyName = true)]
        public String UserName { get; set; }

        [SMA.Parameter(ParameterSetName = "DefaultCredential", ValueFromPipelineByPropertyName = true)]
        public String Password { get; set; }

        [SMA.Parameter(ParameterSetName = "ExplicitCredential", ValueFromPipelineByPropertyName = true)]
        public PSCredential Credential { get; set; }

        protected ClientContext CreateClientContext()
        {
            var clientContext = new ClientContext(Url);

            if (Credential != null)
            {
                clientContext.Credentials = Credential.GetNetworkCredential();
            }
            else
            {
                clientContext.Credentials = CredentialFactory.CreateCredentials(
                    CredentialType,
                    UserName,
                    Password,
                    Url
                );
            }

            return clientContext;
        }
    }
}
